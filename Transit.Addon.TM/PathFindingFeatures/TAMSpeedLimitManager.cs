using System;
using System.Collections.Generic;
using System.Threading;
using ColossalFramework;
using Transit.Addon.TM.Data;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public class TAMSpeedLimitManager : Singleton<TAMSpeedLimitManager>, IRoadSpeedManager
    {
        public List<ushort> AvailableSpeedLimits { get; private set; }

        /// <summary>
        /// For each lane: Defines the currently set speed limit
        /// </summary>
        private IDictionary<uint, ushort> _laneSpeedLimits = null;
        private readonly object _laneSpeedLimitsLock = new object();
        private ushort?[][] _segmentSpeedLimits; // for faster, lock-free access, 1st index: segment id, 2nd index: lane index

        public TAMSpeedLimitManager()
        {
            AvailableSpeedLimits = new List<ushort>();
            AvailableSpeedLimits.Add(10);
            AvailableSpeedLimits.Add(20);
            AvailableSpeedLimits.Add(30);
            AvailableSpeedLimits.Add(40);
            AvailableSpeedLimits.Add(50);
            AvailableSpeedLimits.Add(60);
            AvailableSpeedLimits.Add(70);
            AvailableSpeedLimits.Add(80);
            AvailableSpeedLimits.Add(90);
            AvailableSpeedLimits.Add(100);
            AvailableSpeedLimits.Add(120);
            AvailableSpeedLimits.Add(130);
            AvailableSpeedLimits.Add(0);
        }

        public void Init()
        {
            _laneSpeedLimits = new Dictionary<uint, ushort>();
            _segmentSpeedLimits = new ushort?[Singleton<NetManager>.instance.m_segments.m_size][];
        }

        public void Load(TAMLaneSpeedLimit[] limits)
        {
            if (limits != null)
            {
                foreach (var limit in limits)
                {
                    Load(limit);
                }
            }
        }

        public void Load(TAMLaneSpeedLimit limit)
        {
            if (limit != null)
            {
                if (limit.SpeedLimit != null)
                {
                    SetLaneSpeedLimit(limit.LaneId, limit.SpeedLimit.Value);
                }
            }
        }

        public void Reset()
        {
            _segmentSpeedLimits = null;
            _laneSpeedLimits = null;
        }

        public bool IsLoaded()
        {
            return _laneSpeedLimits != null;
        }

        public TAMLaneSpeedLimit[] GetAllLaneData()
        {
            var list = new List<TAMLaneSpeedLimit>();

            foreach (var kvp in _laneSpeedLimits)
            {
                list.Add(new TAMLaneSpeedLimit()
                {
                    LaneId = kvp.Key,
                    SpeedLimit = kvp.Value
                });
            }

            return list.ToArray();
        }

        /// <summary>
        /// Determines the currently set speed limit for the given segment and lane direction in terms of discrete speed limit levels.
        /// An in-game speed limit of 2.0 (e.g. on highway) is hereby translated into a discrete speed limit value of 100 (km/h).
        /// </summary>
        /// <param name="segmentId"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public ushort GetCustomSpeedLimit(ushort segmentId, NetInfo.Direction dir)
        {
            // calculate the currently set mean speed limit
            if (segmentId == 0)
                return 0;
            if ((Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
                return 0;

            var segmentInfo = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].Info;
            uint curLaneId = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_lanes;
            int laneIndex = 0;
            float meanSpeedLimit = 0f;
            uint validLanes = 0;
            while (laneIndex < segmentInfo.m_lanes.Length && curLaneId != 0u)
            {
                NetInfo.Direction d = segmentInfo.m_lanes[laneIndex].m_direction;
                if ((segmentInfo.m_lanes[laneIndex].m_laneType & (NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle)) == NetInfo.LaneType.None || d != dir)
                    goto nextIter;

                float? setSpeedLimit = GetLaneSpeedLimit(curLaneId);
                if (setSpeedLimit != null)
                    meanSpeedLimit += ToGameSpeedLimit((ushort)setSpeedLimit); // custom speed limit
                else
                    meanSpeedLimit += segmentInfo.m_lanes[laneIndex].m_speedLimit; // game default
                ++validLanes;

                nextIter:
                curLaneId = Singleton<NetManager>.instance.m_lanes.m_buffer[curLaneId].m_nextLane;
                laneIndex++;
            }

            if (validLanes > 0)
                meanSpeedLimit /= (float)validLanes;
            return ToCustomSpeedLimit(meanSpeedLimit);
        }

        /// <summary>
        /// Determines the currently set speed limit for the given lane in terms of discrete speed limit levels.
        /// An in-game speed limit of 2.0 (e.g. on highway) is hereby translated into a discrete speed limit value of 100 (km/h).
        /// </summary>
        /// <param name="laneId"></param>
        /// <returns></returns>
        private ushort GetOrCreateSpeedLimit(uint laneId)
        {
            // check custom speed limit
            float? setSpeedLimit = GetLaneSpeedLimit(laneId);
            if (setSpeedLimit != null)
                return (ushort)setSpeedLimit;

            // check default speed limit
            ushort segmentId = Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_segment;

            if (segmentId == 0)
                return 0;
            if ((Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
                return 0;

            var segmentInfo = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].Info;

            uint curLaneId = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_lanes;
            int laneIndex = 0;
            while (laneIndex < segmentInfo.m_lanes.Length && curLaneId != 0u)
            {
                if (curLaneId == laneId)
                {
                    return ToCustomSpeedLimit(segmentInfo.m_lanes[laneIndex].m_speedLimit);
                }

                laneIndex++;
                curLaneId = Singleton<NetManager>.instance.m_lanes.m_buffer[curLaneId].m_nextLane;
            }

            Log.Warning($"Speed limit for lane {laneId} could not be determined.");
            return 0; // no speed limit found
        }

        /// <summary>
        /// Determines the currently set speed limit for the given lane in terms of game (floating point) speed limit levels
        /// </summary>
        /// <param name="laneId"></param>
        /// <returns></returns>
        private float GetGameSpeedLimit(uint laneId)
        {
            return ToGameSpeedLimit(GetOrCreateSpeedLimit(laneId));
        }

        public float GetLaneSpeedLimit(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedUnitType unitType)
        {
            if ((unitType & TMSupported.UNITS) == 0)
            {
                return laneInfo.m_speedLimit;
            }

            return GetLaneSpeedLimit(segmentId, laneIndex, laneId, laneInfo);
        }

        public float GetLaneSpeedLimit(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo)
        {
            if (Flags.IsInitDone())
            {
                if (_segmentSpeedLimits.Length <= segmentId)
                {
                    Log.Error($"segmentSpeedLimits.Length = {_segmentSpeedLimits.Length}, segmentId={segmentId}. Out of range!");
                    return laneInfo.m_speedLimit;
                }

                ushort?[] fastArray = _segmentSpeedLimits[segmentId];
                if (fastArray != null && 
                    fastArray.Length > laneIndex && 
                    fastArray[laneIndex] != null)
                {
                    return ToGameSpeedLimit((ushort)fastArray[laneIndex]);
                }
                else
                {
                    return laneInfo.m_speedLimit;
                }
            }
            else
                return GetGameSpeedLimit(laneId);
        }

        /// <summary>
        /// Converts a custom speed limit to a game speed limit.
        /// </summary>
        /// <param name="customSpeedLimit"></param>
        /// <returns></returns>
        private float ToGameSpeedLimit(ushort customSpeedLimit)
        {
            if (customSpeedLimit == 0)
                return 4f;
            return (float)customSpeedLimit / 50f;
        }

        /// <summary>
        /// Converts a game speed limit to a custom speed limit.
        /// </summary>
        /// <param name="gameSpeedLimit"></param>
        /// <returns></returns>
        private ushort ToCustomSpeedLimit(float gameSpeedLimit)
        {
            gameSpeedLimit /= 2f; // 1 == 100 km/h

            // translate the floating point speed limit into our discrete version
            ushort speedLimit = 0;
            if (gameSpeedLimit < 0.15f)
                speedLimit = 10;
            else if (gameSpeedLimit < 1.15f)
                speedLimit = (ushort)((ushort)Math.Round(gameSpeedLimit * 10f) * 10u);
            else if (gameSpeedLimit < 1.25f)
                speedLimit = 120;
            else if (gameSpeedLimit < 1.35f)
                speedLimit = 130;

            return speedLimit;
        }

        /// <summary>
        /// Sets the speed limit of a given segment and lane direction.
        /// </summary>
        /// <param name="segmentId"></param>
        /// <param name="dir"></param>
        /// <param name="speedLimit"></param>
        /// <returns></returns>
        public bool SetSegmentSpeedLimit(ushort segmentId, NetInfo.Direction dir, ushort speedLimit)
        {
            if (segmentId == 0)
                return false;
            if (!AvailableSpeedLimits.Contains(speedLimit))
                return false;
            if ((Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
                return false;

            uint curLaneId = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_lanes;
            int laneIndex = 0;
            while (laneIndex < Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].Info.m_lanes.Length && curLaneId != 0u)
            {
                NetInfo.Direction d = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].Info.m_lanes[laneIndex].m_direction;
                if ((Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].Info.m_lanes[laneIndex].m_laneType & (NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle)) == NetInfo.LaneType.None || d != dir)
                    goto nextIter;

                Log._Debug($"SpeedLimitManager: Setting speed limit of lane {curLaneId} to {speedLimit}");
                SetLaneSpeedLimit(curLaneId, speedLimit);

                nextIter:
                curLaneId = Singleton<NetManager>.instance.m_lanes.m_buffer[curLaneId].m_nextLane;
                laneIndex++;
            }

            return true;
        }

        private void SetLaneSpeedLimit(uint laneId, ushort speedLimit)
        {
            if (laneId <= 0)
                return;
            if (((NetLane.Flags)Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags & NetLane.Flags.Created) == NetLane.Flags.None)
                return;

            ushort segmentId = Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_segment;
            if (segmentId <= 0)
                return;
            if ((Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
                return;

            NetInfo segmentInfo = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].Info;
            uint curLaneId = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_lanes;
            uint laneIndex = 0;
            while (laneIndex < segmentInfo.m_lanes.Length && curLaneId != 0u)
            {
                if (curLaneId == laneId)
                {
                    SetLaneSpeedLimit(segmentId, laneIndex, laneId, speedLimit);
                    return;
                }
                laneIndex++;
                curLaneId = Singleton<NetManager>.instance.m_lanes.m_buffer[curLaneId].m_nextLane;
            }
        }

        private void SetLaneSpeedLimit(ushort segmentId, uint laneIndex, uint laneId, ushort speedLimit)
        {
            if (segmentId <= 0 || laneIndex < 0 || laneId <= 0)
                return;
            if ((Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
            {
                return;
            }
            if (((NetLane.Flags)Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags & NetLane.Flags.Created) == NetLane.Flags.None)
                return;
            NetInfo segmentInfo = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].Info;
            if (laneIndex >= segmentInfo.m_lanes.Length)
            {
                return;
            }

            try
            {
                Monitor.Enter(_laneSpeedLimitsLock);
                Log._Debug($"Flags.setLaneSpeedLimit: setting speed limit of lane index {laneIndex} @ seg. {segmentId} to {speedLimit}");

                _laneSpeedLimits[laneId] = speedLimit;

                // save speed limit into the fast-access array.
                // (1) ensure that the array is defined and large enough
                if (_segmentSpeedLimits[segmentId] == null)
                {
                    _segmentSpeedLimits[segmentId] = new ushort?[segmentInfo.m_lanes.Length];
                }
                else if (_segmentSpeedLimits[segmentId].Length < segmentInfo.m_lanes.Length)
                {
                    var oldArray = _segmentSpeedLimits[segmentId];
                    _segmentSpeedLimits[segmentId] = new ushort?[segmentInfo.m_lanes.Length];
                    Array.Copy(oldArray, _segmentSpeedLimits[segmentId], oldArray.Length);
                }
                // (2) insert the custom speed limit
                _segmentSpeedLimits[segmentId][laneIndex] = speedLimit;
            }
            finally
            {
                Monitor.Exit(_laneSpeedLimitsLock);
            }
        }

        private float? GetLaneSpeedLimit(uint laneId)
        {
            try
            {
                Monitor.Enter(_laneSpeedLimitsLock);

                if (laneId <= 0 || _laneSpeedLimits == null || !_laneSpeedLimits.ContainsKey(laneId))
                    return null;

                return _laneSpeedLimits[laneId];
            }
            finally
            {
                Monitor.Exit(_laneSpeedLimitsLock);
            }
        }
    }
}
