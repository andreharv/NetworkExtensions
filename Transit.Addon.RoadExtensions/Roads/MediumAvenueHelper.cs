using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads
{
    public static class MediumAvenueHelper
    {
        public static void Setup50LimitProps(this NetInfo info)
        {
            var speed50 = Prefabs.Find<PropInfo>("50 Speed Limit", false);
            var speed60 = Prefabs.Find<PropInfo>("60 Speed Limit", false);

            if (speed50 == null || speed60 == null)
            {
                return;
            }

            foreach (var lane in info.m_lanes.Where(l => l.m_laneProps != null))
            {
                if (lane.m_laneProps.m_props == null || 
                    lane.m_laneProps.m_props.Length == 0)
                {
                    continue;
                }

                var speed60Prop = lane
                    .m_laneProps
                    .m_props
                    .FirstOrDefault(prop => prop.m_prop == speed60);

                if (speed60Prop != null)
                {
                    var newPropsContent = new List<NetLaneProps.Prop>();
                    var speed50Prop = speed60Prop.ShallowClone();
                    speed50Prop.m_prop = speed50;
                    speed50Prop.m_finalProp = null;

                    newPropsContent.AddRange(lane.m_laneProps.m_props.Where(prop => prop.m_prop != speed60));
                    newPropsContent.Add(speed50Prop);

                    var newProps = ScriptableObject.CreateInstance<NetLaneProps>();
                    newProps.name = lane.m_laneProps.name + "_clone";
                    newProps.m_props = newPropsContent.ToArray();
                    lane.m_laneProps = newProps;
                }
            }
        }
    }
}
