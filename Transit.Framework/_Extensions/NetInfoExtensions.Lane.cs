using System.Linq;
using UnityEngine;

namespace Transit.Framework
{
    public static partial class NetInfoExtensions
    {
        public static NetInfo.Lane Clone(this NetInfo.Lane templateLane, string newName)
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

            var templateLaneProps = templateLane.m_laneProps ?? ScriptableObject.CreateInstance<NetLaneProps>();
            if (templateLaneProps.m_props == null)
            {
                templateLaneProps.m_props = new NetLaneProps.Prop[0];
            }

            newLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
            newLane.m_laneProps.name = newName;
            newLane.m_laneProps.m_props = templateLaneProps
                .m_props
                .Select(p => p.ShallowClone())
                .ToArray();

            return newLane;
        }
    }
}
