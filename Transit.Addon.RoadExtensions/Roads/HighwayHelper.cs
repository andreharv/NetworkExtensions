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

        public static NetInfo.Lane SetHighwayLeftShoulder(this NetInfo hwInfo, NetInfo hwInfoTemplate, NetInfoVersion version)
        {
            var leftHwLaneTemplate = hwInfoTemplate.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.None);
            var leftHwLane = hwInfo.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.None);

            leftHwLane.m_laneProps = leftHwLaneTemplate.m_laneProps.Clone("Highway Left Props");

            leftHwLane.m_width = 2;
            leftHwLane.m_position = hwInfo.m_halfWidth - leftHwLane.m_width;
            leftHwLane.m_position = -leftHwLane.m_position;

            // Default props position
            switch (version)
            {
                case NetInfoVersion.Ground:
                    foreach (var prop in leftHwLane.m_laneProps.m_props)
                    {
                        prop.m_position.x = -1f;
                    }
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    foreach (var prop in leftHwLane.m_laneProps.m_props)
                    {
                        prop.m_position.x = -0.6f;
                    }
                    break;

                case NetInfoVersion.Slope:
                case NetInfoVersion.Tunnel:
                    foreach (var prop in leftHwLane.m_laneProps.m_props)
                    {
                        prop.m_position.x = 1f;
                    }
                    break;
            }

            return leftHwLane;
        }

        public static NetInfo.Lane SetHighwayRightShoulder(this NetInfo hwInfo, NetInfo hwInfoTemplate, NetInfoVersion version)
        {
            var rightHwLaneTemplate = hwInfoTemplate.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.None);
            var rightHwLane = hwInfo.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.None);

            rightHwLane.m_laneProps = rightHwLaneTemplate.m_laneProps.Clone("Highway Right Props");

            rightHwLane.m_width = 2;
            rightHwLane.m_position = hwInfo.m_halfWidth - rightHwLane.m_width;

            // Default props position
            switch (version)
            {
                case NetInfoVersion.Ground:
                    foreach (var prop in rightHwLane.m_laneProps.m_props)
                    {
                        prop.m_position.x = 1f;
                    }
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    foreach (var prop in rightHwLane.m_laneProps.m_props)
                    {
                        prop.m_position.x = 0.6f;
                    }
                    break;

                case NetInfoVersion.Slope:
                case NetInfoVersion.Tunnel:
                    foreach (var prop in rightHwLane.m_laneProps.m_props)
                    {
                        prop.m_position.x = -1f;
                    }
                    break;
            }

            return rightHwLane;
        }

        public static IEnumerable<NetInfo.Lane> SetHighwayVehicleLanes(this NetInfo hwInfo, int lanesToAdd = 0)
        {
            if (lanesToAdd > 0)
            {
                var sourceLane = hwInfo.m_lanes.First(l => l.m_laneType != NetInfo.LaneType.None);
                var tempLanes = hwInfo.m_lanes.ToList();

                for (var i = 0; i < lanesToAdd; i++)
                {
                    var newLane = sourceLane.Clone();
                    tempLanes.Add(newLane);
                }

                hwInfo.m_lanes = tempLanes.ToArray();
            }

            var vehicleLanes = hwInfo.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToArray();

            const float laneWidth = 4f;
            var nbLanes = vehicleLanes.Count();
            var positionStart = laneWidth * ((1f - nbLanes) / 2f);

            for (var i = 0; i < vehicleLanes.Length; i++)
            {
                var l = vehicleLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;
                l.m_verticalOffset = 0f;
                l.m_width = laneWidth;
                l.m_position = positionStart + i * laneWidth;
                l.m_laneProps = l.m_laneProps.Clone();

                foreach (var prop in l.m_laneProps.m_props)
                {
                    prop.m_position = new Vector3(0, 0, 0);
                }
            }

            return vehicleLanes;
        }

        public static IEnumerable<NetLaneProps.Prop> GetLeftTrafficLights(this NetInfo info, NetInfoVersion version)
        {
            if (version == NetInfoVersion.Slope ||
                version == NetInfoVersion.Tunnel)
            {
                yield break;
            }

            foreach (var prop in info
                .FindLane(name => name.Contains("left"))
                .m_laneProps
                .m_props
                .Where(lp => lp.m_prop.name.Contains("Traffic"))
                .Select(prop => prop.ShallowClone()))
            {
                if (version == NetInfoVersion.Elevated || 
                    version == NetInfoVersion.Bridge)
                {
                    prop.m_position = new Vector3(-1.75f, 1, 0);
                }
                else
                {
                    prop.m_position.x = -1;
                }

                yield return prop;
            }
        }

        public static IEnumerable<NetLaneProps.Prop> GetRightTrafficLights(this NetInfo info, NetInfoVersion version)
        {
            if (version == NetInfoVersion.Slope ||
                version == NetInfoVersion.Tunnel)
            {
                yield break;
            }

            foreach (var prop in info
                .FindLane(name => name.Contains("right"))
                .m_laneProps
                .m_props
                .Where(lp => lp.m_prop.name.Contains("Traffic"))
                .Select(prop => prop.ShallowClone()))
            {
                if (version == NetInfoVersion.Elevated ||
                    version == NetInfoVersion.Bridge)
                {
                    prop.m_position = new Vector3(1.75f, 1, 0);
                }
                else
                {
                    prop.m_position.x = 1;
                }

                yield return prop;
            }
        }

        public static void SetLights(this ICollection<NetLaneProps.Prop> props, NetInfoVersion version)
        {
            var streetLightPropInfo = Prefabs.Find<PropInfo>("New Street Light Highway", false);
            if (streetLightPropInfo == null)
            {
                return;
            }

            var streetLightProp = props.FirstOrDefault(prop => prop.m_prop == streetLightPropInfo);
            if (streetLightProp == null)
            {
                return;
            }

            switch (version)
            {
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    streetLightProp.m_repeatDistance = 80;
                    streetLightProp.m_position = new Vector3(1.75f, -2, 0);
                    break;

                case NetInfoVersion.Tunnel:
                    streetLightProp.m_repeatDistance = 40;
                    streetLightProp.m_segmentOffset = 0;
                    streetLightProp.m_position = new Vector3(3.2f, -4.5f, 0);
                    break;

                case NetInfoVersion.Slope:
                    props.Trim(p => p.m_prop == streetLightPropInfo);
                    break;
            }
        }

        public static void AddLeftWallLights(this ICollection<NetLaneProps.Prop> props, int xPos = 0)
        {
            var wallLightPropInfo = Prefabs.Find<PropInfo>("Wall Light Orange");
            var wallLightProp = new NetLaneProps.Prop();
            wallLightProp.m_prop = wallLightPropInfo.ShallowClone();
            wallLightProp.m_probability = 100;
            wallLightProp.m_repeatDistance = 20;
            wallLightProp.m_segmentOffset = 0;
            wallLightProp.m_angle = 270;
            wallLightProp.m_position = new Vector3(xPos, 1.5f, 0);
            props.Add(wallLightProp);
        }

        public static void AddRightWallLights(this ICollection<NetLaneProps.Prop> props, int xPos = 0)
        {
            var wallLightPropInfo = Prefabs.Find<PropInfo>("Wall Light Orange");
            var wallLightProp = new NetLaneProps.Prop();
            wallLightProp.m_prop = wallLightPropInfo.ShallowClone();
            wallLightProp.m_probability = 100;
            wallLightProp.m_repeatDistance = 20;
            wallLightProp.m_segmentOffset = 0;
            wallLightProp.m_angle = 90;
            wallLightProp.m_position = new Vector3(xPos, 1.5f, 0);
            props.Add(wallLightProp);
        }

        public static void SetHighwaySignsSlope(this ICollection<NetLaneProps.Prop> props)
        {
            var speedLimit = Prefabs.Find<PropInfo>("100 Speed Limit");
            var motorwaySign = Prefabs.Find<PropInfo>("Motorway Sign");

            foreach (var p in props)
            {
                if (p.m_prop == speedLimit)
                {
                    p.m_position = new Vector3(0f, 1f, 10f);
                }

                if (p.m_prop == motorwaySign)
                {
                    p.m_position = new Vector3(0f, 1f, 1f);
                }
            }
        }

        public static void TrimNonHighwayProps(this NetInfo info, bool removeRightStreetLights = false)
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
