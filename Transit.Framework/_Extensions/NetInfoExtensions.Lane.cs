using System.Linq;
using UnityEngine;

namespace Transit.Framework
{
    public static partial class NetInfoExtensions
    {
        public static NetInfo.Lane Clone(this NetInfo.Lane templateLane, string newName = null)
        {
            var newLane = new NetInfo.Lane();

            newLane.m_direction = templateLane.m_direction;
            newLane.m_finalDirection = templateLane.m_finalDirection;
            newLane.m_allowConnect = templateLane.m_allowConnect;
            newLane.m_allowStop = false;
            newLane.m_stopOffset = 0;
            newLane.m_laneType = templateLane.m_laneType;
            newLane.m_speedLimit = templateLane.m_speedLimit;
            newLane.m_vehicleType = templateLane.m_vehicleType;
            newLane.m_verticalOffset = templateLane.m_verticalOffset;
            newLane.m_width = templateLane.m_width;
            newLane.m_laneProps = templateLane.m_laneProps.Clone(newName);

            return newLane;
        }

        public static NetInfo.Lane ClonePedLane(this NetInfo.Lane lane)
        {
            return new NetInfo.Lane()
            {
                m_position = lane.m_position,
                m_width = lane.m_width,
                m_verticalOffset = lane.m_verticalOffset,
                m_stopOffset = lane.m_stopOffset,
                m_speedLimit = lane.m_speedLimit,
                m_direction = lane.m_direction,
                m_laneType = lane.m_laneType,
                m_vehicleType = lane.m_vehicleType,
                m_laneProps = lane.m_laneProps,
                m_allowStop = lane.m_allowStop,
                m_useTerrainHeight = lane.m_useTerrainHeight
            };
        }
    }
}
