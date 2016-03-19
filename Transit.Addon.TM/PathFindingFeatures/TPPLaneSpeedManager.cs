using System;
using System.Collections.Generic;
using ColossalFramework;
using Transit.Addon.TM.Data;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public class TPPLaneSpeedManager : Singleton<TPPLaneSpeedManager>
    {
        private TAMLaneSpeedLimit[] _laneData = null;

        public void Init(TAMLaneSpeedLimit[] laneData)
        {
            _laneData = laneData;
            if (_laneData == null)
            {
                _laneData = new TAMLaneSpeedLimit[NetManager.MAX_LANE_COUNT];
            }

            foreach (TAMLaneSpeedLimit data in _laneData)
            {
                if (data == null)
                    continue;

                if (data.SpeedLimit == 0)
                {
                    NetSegment segment = NetManager.instance.m_segments.m_buffer[NetManager.instance.m_lanes.m_buffer[data.LaneId].m_segment];
                    NetInfo info = segment.Info;
                    uint l = segment.m_lanes;
                    int n = 0;
                    while (l != data.LaneId && n < info.m_lanes.Length)
                    {
                        l = NetManager.instance.m_lanes.m_buffer[l].m_nextLane;
                        n++;
                    }

                    if (n < info.m_lanes.Length)
                        data.SpeedLimit = info.m_lanes[n].m_speedLimit;
                }
            }
        }

        public void Reset()
        {
            _laneData = null;
        }

        public bool IsLoaded()
        {
            return _laneData != null;
        }

        public TAMLaneSpeedLimit[] GetAllLaneData()
        {
            return _laneData;
        }

        public TAMLaneSpeedLimit CreateLaneData(uint laneId)
        {
            var laneData = new TAMLaneSpeedLimit()
            {
                LaneId = laneId
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
                laneData.SpeedLimit = netInfo.m_lanes[laneIndex].m_speedLimit;
            }

            _laneData[laneId] = laneData;

            return laneData;
        }

        /// <summary>
        /// Gets lane data for the given lane id.
        /// The result may be null.
        /// Warning: This method is not thread-safe.
        /// </summary>
        /// <param name="laneId"></param>
        /// <returns></returns>
        public TAMLaneSpeedLimit GetLaneData(uint laneId)
        {
            return _laneData[laneId];
        }

        /// <summary>
        /// Gets lane data for the given lane id. Create it if does not exist.
        /// Warning: This method is not thread-safe.
        /// </summary>
        /// <param name="laneId"></param>
        /// <returns></returns>
        public TAMLaneSpeedLimit GetOrCreateLane(uint laneId)
        {
            TAMLaneSpeedLimit lane = _laneData[laneId];
            if (lane == null)
                lane = CreateLaneData(laneId);

            return lane;
        }

        // TODO: deprecate this
        public float GetLaneSpeed(uint laneId, NetInfo.Lane laneInfo)
        {
            if ((ToolModuleV2.ActiveOptions & Options.RoadCustomizerTool) != Options.RoadCustomizerTool)
            {
                return laneInfo.m_speedLimit;
            }

            TAMLaneSpeedLimit lane = GetLaneData(laneId);
			if (lane == null)
				return laneInfo.m_speedLimit;

            return lane.SpeedLimit;
        }

        public float GetLaneSpeedRestriction(uint laneId, NetInfo.Lane laneInfo=null)
        {
            TAMLaneSpeedLimit lane = GetLaneData(laneId);
			if (lane == null) {
				if (laneInfo == null)
					laneInfo = NetManager.instance.GetLaneInfo(laneId);
				if (laneInfo == null)
					return 2f;
				return laneInfo.m_speedLimit;
			}

			return lane.SpeedLimit;
		}

        public void SetLaneSpeedRestriction(uint laneId, int speed)
        {
            GetOrCreateLane(laneId).SpeedLimit = (float)Math.Round(speed/50f, 2);
        }
    }
}
