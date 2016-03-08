using System;
using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.Data
{
    public static class TPPLaneDataManager
    {
        internal static TPPLaneData[] sm_lanes = null;

        public static TPPLaneData CreateLane(uint laneId)
        {
            TPPLaneData lane = new TPPLaneData()
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
                    lane.m_vehicleTypes = netInfoLane.AllowedVehicleTypes;

                lane.m_speed = netInfo.m_lanes[laneIndex].m_speedLimit;
            }

            NetManager.instance.m_lanes.m_buffer[laneId].m_flags |= TPPLaneData.CONTROL_BIT;

            sm_lanes[laneId] = lane;

            return lane;
        }

        public static TPPLaneData GetLane(uint laneId)
        {
            TPPLaneData lane = sm_lanes[laneId];
            if (lane == null || (NetManager.instance.m_lanes.m_buffer[laneId].m_flags & TPPLaneData.CONTROL_BIT) == 0)
                lane = CreateLane(laneId);

            return lane;
        }

        #region Lane Connections
        public static bool AddLaneConnection(uint laneId, uint connectionId)
        {
            TPPLaneData lane = GetLane(laneId);
            GetLane(connectionId); // makes sure lane information is stored

            return lane.AddConnection(connectionId);
        }

        public static bool RemoveLaneConnection(uint laneId, uint connectionId)
        {
            TPPLaneData lane = GetLane(laneId);

            return lane.RemoveConnection(connectionId);
        }

        public static uint[] GetLaneConnections(uint laneId)
        {
            TPPLaneData lane = GetLane(laneId);

            return lane.GetConnectionsAsArray();
        }

        private const ExtendedVehicleType sm_routedUnits =
            ExtendedVehicleType.ServiceVehicles |
            ExtendedVehicleType.PassengerCar |
            ExtendedVehicleType.CargoTruck |
            ExtendedVehicleType.Bus |
            ExtendedVehicleType.Taxi;

        public static bool CheckLaneConnection(this NetInfo.Lane laneInfo, ExtendedVehicleType vehicleType, uint from, uint to)
        {
            if ((vehicleType & sm_routedUnits) == 0)
            {
                return true;
            }

            if ((laneInfo.m_vehicleType & VehicleInfo.VehicleType.Car) == VehicleInfo.VehicleType.None)
            {
                return true;
            }

            // Quick fix for tram
            if ((laneInfo.m_vehicleType & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None)
            {
                return true;
            }

            TPPLaneData lane = GetLane(from);

            return lane.ConnectsTo(to);
        }
        #endregion

        #region Vehicle Restrictions
        public static bool CanUseLane(this NetInfo.Lane laneInfo, ExtendedVehicleType vehicleType, uint laneId)
        {
            if ((vehicleType & sm_routedUnits) == 0)
            {
                return true;
            }

            if ((laneInfo.m_vehicleType & VehicleInfo.VehicleType.Car) == VehicleInfo.VehicleType.None)
            {
                return true;
            }

            // Quick fix for tram
            if ((laneInfo.m_vehicleType & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None)
            {
                return true;
            }

            return (GetLane(laneId).m_vehicleTypes & vehicleType) != ExtendedVehicleType.None;
        }

        public static ExtendedVehicleType GetVehicleRestrictions(uint laneId)
        {
            return GetLane(laneId).m_vehicleTypes;
        }

        public static void SetVehicleRestrictions(uint laneId, ExtendedVehicleType vehicleRestrictions)
        {
            GetLane(laneId).m_vehicleTypes = vehicleRestrictions;
        }

        public static void ToggleVehicleRestriction(uint laneId, ExtendedVehicleType vehicleType)
        {
            GetLane(laneId).m_vehicleTypes ^= vehicleType;
        }

        #endregion

        #region Lane Speeds

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
