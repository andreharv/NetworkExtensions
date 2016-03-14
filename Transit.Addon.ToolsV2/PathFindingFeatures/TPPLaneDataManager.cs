using System;
using Transit.Addon.ToolsV2.Data;
using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.PathFindingFeatures
{
    public static class TPPLaneDataManager
    {
        internal static TPPLaneDataV2[] sm_lanes = null;

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

            NetManager.instance.m_lanes.m_buffer[laneId].m_flags |= TPPLaneDataV2.CONTROL_BIT;

            sm_lanes[laneId] = lane;

            return lane;
        }

        public static TPPLaneDataV2 GetLane(uint laneId)
        {
            TPPLaneDataV2 lane = sm_lanes[laneId];
            if (lane == null || (NetManager.instance.m_lanes.m_buffer[laneId].m_flags & TPPLaneDataV2.CONTROL_BIT) == 0)
                lane = CreateLane(laneId);

            return lane;
        }

        #region Lane Connections
        public static bool AddLaneConnection(uint laneId, uint connectionId)
        {
            TPPLaneDataV2 lane = GetLane(laneId);
            GetLane(connectionId); // makes sure lane information is stored

            return lane.AddConnection(connectionId);
        }

        public static bool RemoveLaneConnection(uint laneId, uint connectionId)
        {
            TPPLaneDataV2 lane = GetLane(laneId);

            return lane.RemoveConnection(connectionId);
        }

        public static uint[] GetLaneConnections(uint laneId)
        {
            TPPLaneDataV2 lane = GetLane(laneId);

            return lane.GetConnectionsAsArray();
        }
        #endregion

        #region Vehicle Restrictions

        public static ExtendedUnitType GetVehicleRestrictions(uint laneId)
        {
            return GetLane(laneId).m_unitTypes;
        }

        public static void SetVehicleRestrictions(uint laneId, ExtendedUnitType vehicleRestrictions)
        {
            GetLane(laneId).m_unitTypes = vehicleRestrictions;
        }

        public static void ToggleVehicleRestriction(uint laneId, ExtendedUnitType vehicleType)
        {
            GetLane(laneId).m_unitTypes ^= vehicleType;
        }

        #endregion

        #region Lane Speeds

        // TODO: deprecate this
        public static float GetLaneSpeed(uint laneId, NetInfo.Lane lane)
        {
            if ((ToolModuleV2.ActiveOptions & ModOptions.RoadCustomizerTool) != ModOptions.RoadCustomizerTool)
            {
                return lane.m_speedLimit;
            }

            return GetLane(laneId).m_speed;
        }

        public static float GetLaneSpeedRestriction(uint laneId)
        {
            return GetLane(laneId).m_speed;
        }

        public static void SetLaneSpeedRestriction(uint laneId, int speed)
        {
            GetLane(laneId).m_speed = (float)Math.Round(speed/50f, 2);
        }

        #endregion
    }
}
