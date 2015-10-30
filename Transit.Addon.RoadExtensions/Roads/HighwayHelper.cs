using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads
{
    public static class HighwayHelper
    {
        [Obsolete("Use SetHighwayLeftShoulder and SetHighwayRightShoulder")]
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


        public static NetInfo DisableHighwayParkingsAndPeds(this NetInfo hwInfo)
        {
            // Removing Parking lanes
            hwInfo.m_lanes = hwInfo
                .m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.Parking)
                .ToArray();

            // Transforming Pedestrian into None lanes
            foreach (var lane in hwInfo.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian))
            {
                lane.m_laneType = NetInfo.LaneType.None;
            }

            return hwInfo;
        }

        public static NetInfo.Lane SetHighwayLeftShoulder(this NetInfo hwInfo, NetInfo hwInfoTemplate)
        {
            var leftHwLaneTemplate = hwInfoTemplate.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.None);
            var leftHwLane = hwInfo.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.None);

            leftHwLane.m_laneProps = leftHwLaneTemplate.m_laneProps.Clone("Highway Left Props");

            return leftHwLane;
        }

        public static NetInfo.Lane SetHighwayRightShoulder(this NetInfo hwInfo, NetInfo hwInfoTemplate)
        {
            var rightHwLaneTemplate = hwInfoTemplate.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.None);
            var rightHwLane = hwInfo.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.None);

            rightHwLane.m_laneProps = rightHwLaneTemplate.m_laneProps.Clone("Highway Right Props");

            return rightHwLane;
        }

        public static void TrimHighwayProps(this NetInfo info, bool removeRightStreetLights = false)
        {
            var randomProp = Prefabs.Find<PropInfo>("Random Street Prop", false);
            var streetLight = Prefabs.Find<PropInfo>("New Street Light", false);
            var streetLightHw = Prefabs.Find<PropInfo>("New Street Light Highway", false);
            var manhole = Prefabs.Find<PropInfo>("Manhole", false);

            foreach (var laneProps in info.m_lanes.Select(l => l.m_laneProps).Where(lpi => lpi != null))
            {
                var remainingProp = new List<NetLaneProps.Prop>();

                foreach (var prop in laneProps.m_props.Where(p => p.m_prop != null))
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

                    if (prop.m_prop == streetLightHw &&
                        laneProps.name.Contains("Left"))
                    {
                        continue;
                    }

                    if (removeRightStreetLights)
                    {
                        if (prop.m_prop == streetLight &&
                            laneProps.name.Contains("Right"))
                        {
                            continue;
                        }

                        if (prop.m_prop == streetLightHw &&
                            laneProps.name.Contains("Right"))
                        {
                            continue;
                        }
                    }

                    remainingProp.Add(prop);
                }

                laneProps.m_props = remainingProp.ToArray();
            }
        }
    }
}
