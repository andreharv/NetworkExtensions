using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transit.Framework;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Addon.RoadExtensions.Roads
{
    public static class HighwayHelper
    {
        public static void SetHighwayProps(this NetInfo info, NetInfo highwayInfoTemplate)
        {
            var leftHwLane = highwayInfoTemplate
                .m_lanes
                .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                .FirstOrDefault(l => l.m_laneProps.name.ToLower().Contains("left"));

            var rightHwLane = highwayInfoTemplate
                .m_lanes
                .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                .FirstOrDefault(l => l.m_laneProps.name.ToLower().Contains("right"));

            foreach (var lane in info.m_lanes)
            {
                if (lane.m_laneProps != null && lane.m_laneProps.name != null)
                {
                    if (leftHwLane != null)
                    {
                        if (lane.m_laneProps.name.ToLower().Contains("left"))
                        {
                            var newProps = ScriptableObject.CreateInstance<NetLaneProps>();
                            newProps.name = "Highway6L Left Props";

                            newProps.m_props = leftHwLane
                                .m_laneProps
                                .m_props
                                .Select(p => p.ShallowClone())
                                .ToArray();

                            lane.m_laneProps = newProps;
                        }
                    }

                    if (rightHwLane != null)
                    {
                        if (lane.m_laneProps.name.ToLower().Contains("right"))
                        {
                            var newProps = ScriptableObject.CreateInstance<NetLaneProps>();
                            newProps.name = "Highway6L Right Props";

                            newProps.m_props = rightHwLane
                                .m_laneProps
                                .m_props
                                .Select(p => p.ShallowClone())
                                .ToArray();

                            lane.m_laneProps = newProps;
                        }
                    }
                }
            }
        }

        public static void TrimHighwayProps(this NetInfo info)
        {
            var randomProp = Prefabs.Find<PropInfo>("Random Street Prop", false);
            var streetLight = Prefabs.Find<PropInfo>("New Street Light", false);
            var manhole = Prefabs.Find<PropInfo>("Manhole", false);

            if (randomProp == null)
            {
                return;
            }

            foreach (var laneProps in info.m_lanes.Select(l => l.m_laneProps).Where(lpi => lpi != null))
            {
                var remainingProp = new List<NetLaneProps.Prop>();

                foreach (var prop in laneProps.m_props)
                {
                    if (prop.m_prop != null)
                    {
                        if (prop.m_prop == randomProp)
                        {
                            continue;
                        }

                        if (prop.m_prop == manhole)
                        {
                            continue;
                        }

                        if (prop.m_prop == streetLight &&
                            laneProps.name.Contains("Left"))
                        {
                            continue;
                        }

                        remainingProp.Add(prop);
                    }
                }

                laneProps.m_props = remainingProp.ToArray();
            }
        }
    }
}
