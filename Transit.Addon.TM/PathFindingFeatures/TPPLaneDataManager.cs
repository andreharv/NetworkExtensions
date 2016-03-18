using System;
using Transit.Addon.TM.Data;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public static class TPPLaneDataManager
    {
        internal static TPPLaneDataV2[] sm_lanes = null;
		private static readonly uint[] NO_CONNECTIONS = new uint[0];

        public static TPPLaneDataV2 CreateLane(uint laneId)
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

            sm_lanes[laneId] = lane;

            return lane;
        }

		/// <summary>
		/// Gets lane data for the given lane id. If `create` is true the lane is created if it does not exist.
		/// If `create` is false, the result may be null.
		/// Warning: This method is not thread-safe.
		/// </summary>
		/// <param name="laneId"></param>
		/// <returns></returns>
		public static TPPLaneDataV2 GetLane(uint laneId, bool create)
        {
            TPPLaneDataV2 lane = sm_lanes[laneId];
            if (lane == null && create)
                lane = CreateLane(laneId);

            return lane;
        }

        #region Lane Connections
        public static bool AddLaneConnection(uint fromLaneId, uint toLaneId)
        {
            TPPLaneDataV2 lane = GetLane(fromLaneId, true);
            GetLane(toLaneId, true); // makes sure lane information is stored

            return lane.AddConnection(toLaneId);
        }

        public static bool RemoveLaneConnection(uint laneId, uint connectionId)
        {
            TPPLaneDataV2 lane = GetLane(laneId, false);
			if (lane == null)
				return false;

            return lane.RemoveConnection(connectionId);
        }

        public static uint[] GetLaneConnections(uint laneId)
        {
            TPPLaneDataV2 lane = GetLane(laneId, false);

			if (lane == null)
				return NO_CONNECTIONS;
			return lane.GetConnectionsAsArray();
        }
        #endregion

        #region Vehicle Restrictions

        public static ExtendedUnitType GetVehicleRestrictions(uint laneId)
        {
			TPPLaneDataV2 lane = GetLane(laneId, false);
			if (lane == null)
				return ExtendedUnitType.RoadVehicle; // TODO is this default value saved anywhere?
			return lane.m_unitTypes;
        }

        public static void SetVehicleRestrictions(uint laneId, ExtendedUnitType vehicleRestrictions)
        {
			GetLane(laneId, true).m_unitTypes = vehicleRestrictions;
        }

        public static void ToggleVehicleRestriction(uint laneId, ExtendedUnitType vehicleType)
        {
            GetLane(laneId, true).m_unitTypes ^= vehicleType;
        }

        #endregion

        #region Lane Speeds

        // TODO: deprecate this
        public static float GetLaneSpeed(uint laneId, NetInfo.Lane laneInfo)
        {
            if ((ToolModuleV2.ActiveOptions & Options.RoadCustomizerTool) != Options.RoadCustomizerTool)
            {
                return laneInfo.m_speedLimit;
            }

			TPPLaneDataV2 lane = GetLane(laneId, false);
			if (lane == null)
				return laneInfo.m_speedLimit;

            return lane.m_speed;
        }

        public static float GetLaneSpeedRestriction(uint laneId, NetInfo.Lane laneInfo=null)
        {
			TPPLaneDataV2 lane = GetLane(laneId, false);
			if (lane == null) {
				if (laneInfo == null)
					laneInfo = NetManager.instance.GetLaneInfo(laneId);
				if (laneInfo == null)
					return 2f;
				return laneInfo.m_speedLimit;
			}

			return lane.m_speed;
		}

        public static void SetLaneSpeedRestriction(uint laneId, int speed)
        {
            GetLane(laneId, true).m_speed = (float)Math.Round(speed/50f, 2);
        }

        #endregion
    }
}
