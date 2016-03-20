using System.Linq;
using Transit.Framework.Network;
using UnityEngine;

namespace Transit.Framework
{
    public static partial class NetInfoExtensions
    {
        public static NetInfo.Lane CloneWithoutStops(this NetInfo.Lane templateLane, string newName = null)
        {
            var newLane = new NetInfo.Lane();

            newLane.m_direction = templateLane.m_direction;
            newLane.m_finalDirection = templateLane.m_finalDirection;
            newLane.m_allowConnect = templateLane.m_allowConnect;
            newLane.m_stopType = VehicleInfo.VehicleType.None;
            newLane.m_stopOffset = 0;
            newLane.m_laneType = templateLane.m_laneType;
            newLane.m_speedLimit = templateLane.m_speedLimit;
            newLane.m_vehicleType = templateLane.m_vehicleType;
            newLane.m_verticalOffset = templateLane.m_verticalOffset;
            newLane.m_width = templateLane.m_width;
            newLane.m_laneProps = templateLane.m_laneProps.Clone(newName);

            return newLane;
        }

        public static ExtendedUnitType GetUnitType(this NetInfo.Lane laneInfo)
        {
            if (IsRoadLane(laneInfo))
                return ExtendedUnitType.RoadVehicle;
            else if (IsRailLane(laneInfo))
                return ExtendedUnitType.Train;
            else
                return ExtendedUnitType.None;
        }

        public static bool IsRailLane(this NetInfo.Lane laneInfo)
        {
            return (laneInfo.m_vehicleType & VehicleInfo.VehicleType.Train) != VehicleInfo.VehicleType.None;
        }

        public static bool IsRoadLane(this NetInfo.Lane laneInfo)
        {
            return (laneInfo.m_vehicleType & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None;
        }
    }
}
