using System;
using System.Collections.Generic;
using ColossalFramework;
using Transit.Addon.TM.Data;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public class TPPDataManager : Singleton<TPPDataManager>
    {
        private TPPLaneDataV2[] _lanesData = null;
		private readonly uint[] NO_CONNECTIONS = new uint[0];

        public void Init(TPPLaneDataV2[] laneData)
        {
            _lanesData = laneData;

            if (_lanesData == null)
            {
                _lanesData = new TPPLaneDataV2[NetManager.MAX_LANE_COUNT];
            }

            foreach (TPPLaneDataV2 lane in _lanesData)
            {
                if (lane == null)
                    continue;

                lane.UpdateArrows();

                if (lane.m_speed == 0)
                {
                    NetSegment segment = NetManager.instance.m_segments.m_buffer[NetManager.instance.m_lanes.m_buffer[lane.m_laneId].m_segment];
                    NetInfo info = segment.Info;
                    uint l = segment.m_lanes;
                    int n = 0;
                    while (l != lane.m_laneId && n < info.m_lanes.Length)
                    {
                        l = NetManager.instance.m_lanes.m_buffer[l].m_nextLane;
                        n++;
                    }

                    if (n < info.m_lanes.Length)
                        lane.m_speed = info.m_lanes[n].m_speedLimit;
                }

            }
        }

        public void Reset()
        {
            _lanesData = null;
        }

        public bool IsLoaded()
        {
            return _lanesData != null;
        }

        public TPPLaneDataV2[] GetAllLanes()
        {
            return _lanesData;
        }

        public TPPLaneDataV2 CreateLane(uint laneId)
        {
            TPPLaneDataV2 lane = new TPPLaneDataV2()
            {
                m_laneId = laneId
            };

            NetSegment segment = NetManager.instance.m_segments.m_buffer[NetManager.instance.m_lanes.m_buffer[laneId].m_segment];
            NetInfo netInfo = segment.Info;
            int laneCount = netInfo.m_lanes.Length;
            int laneIndex = 0;
            for (uint l = segment.m_lanes; laneIndex < laneCount && l != 0; laneIndex++)
            {
                if (l == laneId)
                    break;

                l = NetManager.instance.m_lanes.m_buffer[l].m_nextLane;
            }

            if (laneIndex < laneCount)
            {
                ExtendedNetInfoLane netInfoLane = netInfo.m_lanes[laneIndex] as ExtendedNetInfoLane;
                if (netInfoLane != null)
                    lane.m_unitTypes = netInfoLane.AllowedVehicleTypes;

                lane.m_speed = netInfo.m_lanes[laneIndex].m_speedLimit;
            }

            _lanesData[laneId] = lane;

            return lane;
        }

        /// <summary>
        /// Gets lane data for the given lane id.
        /// The result may be null.
        /// Warning: This method is not thread-safe.
        /// </summary>
        /// <param name="laneId"></param>
        /// <returns></returns>
        public TPPLaneDataV2 GetLane(uint laneId)
        {
            return _lanesData[laneId];
        }

        /// <summary>
        /// Gets lane data for the given lane id. Create it if does not exist.
        /// Warning: This method is not thread-safe.
        /// </summary>
        /// <param name="laneId"></param>
        /// <returns></returns>
        public TPPLaneDataV2 GetOrCreateLane(uint laneId)
        {
            TPPLaneDataV2 lane = _lanesData[laneId];
            if (lane == null)
                lane = CreateLane(laneId);

            return lane;
        }

        #region Lane Connections
        public bool AddLaneConnection(uint fromLaneId, uint toLaneId)
        {
            TPPLaneDataV2 lane = GetOrCreateLane(fromLaneId);
            GetOrCreateLane(toLaneId); // makes sure lane information is stored

            return lane.AddConnection(toLaneId);
        }

        public bool RemoveLaneConnection(uint laneId, uint connectionId)
        {
            TPPLaneDataV2 lane = GetLane(laneId);
			if (lane == null)
				return false;

            return lane.RemoveConnection(connectionId);
        }

        public uint[] GetLaneConnections(uint laneId)
        {
            TPPLaneDataV2 lane = GetLane(laneId);

			if (lane == null)
				return NO_CONNECTIONS;
			return lane.GetConnectionsAsArray();
        }
        #endregion

        #region Vehicle Restrictions

        public ExtendedUnitType GetVehicleRestrictions(uint laneId)
        {
			TPPLaneDataV2 lane = GetLane(laneId);
			if (lane == null)
				return ExtendedUnitType.RoadVehicle; // TODO is this default value saved anywhere?
			return lane.m_unitTypes;
        }

        public void SetVehicleRestrictions(uint laneId, ExtendedUnitType vehicleRestrictions)
        {
            GetOrCreateLane(laneId).m_unitTypes = vehicleRestrictions;
        }

        public void ToggleVehicleRestriction(uint laneId, ExtendedUnitType vehicleType)
        {
            GetOrCreateLane(laneId).m_unitTypes ^= vehicleType;
        }

        #endregion

        #region Lane Speeds

        // TODO: deprecate this
        public float GetLaneSpeed(uint laneId, NetInfo.Lane laneInfo)
        {
            if ((ToolModuleV2.ActiveOptions & Options.RoadCustomizerTool) != Options.RoadCustomizerTool)
            {
                return laneInfo.m_speedLimit;
            }

			TPPLaneDataV2 lane = GetLane(laneId);
			if (lane == null)
				return laneInfo.m_speedLimit;

            return lane.m_speed;
        }

        public float GetLaneSpeedRestriction(uint laneId, NetInfo.Lane laneInfo=null)
        {
			TPPLaneDataV2 lane = GetLane(laneId);
			if (lane == null) {
				if (laneInfo == null)
					laneInfo = NetManager.instance.GetLaneInfo(laneId);
				if (laneInfo == null)
					return 2f;
				return laneInfo.m_speedLimit;
			}

			return lane.m_speed;
		}

        public void SetLaneSpeedRestriction(uint laneId, int speed)
        {
            GetOrCreateLane(laneId).m_speed = (float)Math.Round(speed/50f, 2);
        }

        #endregion
    }
}
