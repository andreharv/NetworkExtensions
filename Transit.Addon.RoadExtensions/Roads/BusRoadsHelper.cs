using System.Linq;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads
{
    public static class BusRoadsHelper
    {
        public static void SetBusLaneProps(this NetInfo.Lane lane)
        {
            var prop = Prefabs.Find<PropInfo>("BusLaneText", false);

            if (prop == null)
            {
                return;
            }

            if (lane.m_laneProps == null)
            {
                lane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                lane.m_laneProps.m_props = new NetLaneProps.Prop[] { };
            }
            else
            {
                lane.m_laneProps = lane.m_laneProps.Clone();
            }

            lane.m_laneProps.m_props = lane
                .m_laneProps
                .m_props
                    .Union(new NetLaneProps.Prop
                    {
                        m_prop = prop,
                        m_position = new Vector3(0f, 0f, 7.5f),
                        m_angle = 180f,
                        m_segmentOffset = -1f,
                        m_minLength = 8f,
                        m_startFlagsRequired = NetNode.Flags.Junction
                    })
                .ToArray();
        }
    }
}
