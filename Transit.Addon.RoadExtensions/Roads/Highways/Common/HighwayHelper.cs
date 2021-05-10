﻿using System.Collections.Generic;
using System.Linq;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.Highways.Common
{
    public static class HighwayHelper
    {
        public static NetInfo SetupHighwayLanes(this NetInfo hwInfo)
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
            var leftHwLaneTemplate = hwInfoTemplate.m_lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.None);
            var leftHwLane = hwInfo.m_lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.None);
            if (leftHwLane != null)
            {
                leftHwLane.m_width = 2;
                leftHwLane.m_position = hwInfo.m_halfWidth - leftHwLane.m_width;
                leftHwLane.m_position = -leftHwLane.m_position;
                leftHwLane.m_laneProps = leftHwLaneTemplate?.m_laneProps.Clone("Highway Left Props");
                if (leftHwLane.m_laneProps != null) {
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
                }
            }
            return leftHwLane;
        }

        public static NetInfo.Lane SetHighwayRightShoulder(this NetInfo hwInfo, NetInfo hwInfoTemplate, NetInfoVersion version)
        {
            var rightHwLaneTemplate = hwInfoTemplate.m_lanes.LastOrDefault(l => l.m_laneType == NetInfo.LaneType.None);
            var rightHwLane = hwInfo.m_lanes.LastOrDefault(l => l.m_laneType == NetInfo.LaneType.None);
            if (rightHwLane != null)
            {
                rightHwLane.m_width = 2;
                rightHwLane.m_position = hwInfo.m_halfWidth - rightHwLane.m_width;
                rightHwLane.m_laneProps = rightHwLaneTemplate?.m_laneProps.Clone("Highway Right Props");
                if (rightHwLane.m_laneProps != null)
                {
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
                }
            }
            return rightHwLane;
        }

        public static IEnumerable<NetInfo.Lane> SetHighwayVehicleLanes(this NetInfo hwInfo, int lanesToAdd = 0, bool isTwoWay = false)
        {
            if (lanesToAdd < 0)
            {
                var remainingLanes = new List<NetInfo.Lane>();
                remainingLanes.AddRange(hwInfo
                    .m_lanes
                    .Where(l => l.m_laneType == NetInfo.LaneType.None));
                remainingLanes.AddRange(hwInfo
                    .m_lanes
                    .Where(l => l.m_laneType != NetInfo.LaneType.None)
                    .Skip(-lanesToAdd));

                hwInfo.m_lanes = remainingLanes.ToArray();
            }
            else if (lanesToAdd > 0)
            {
                var sourceLane = hwInfo.m_lanes.First(l => l.m_laneType != NetInfo.LaneType.None);
                var tempLanes = hwInfo.m_lanes.ToList();

                for (var i = 0; i < lanesToAdd; i++)
                {
                    var newLane = sourceLane.CloneWithoutStops();
                    tempLanes.Add(newLane);
                }

                hwInfo.m_lanes = tempLanes.ToArray();
            }

            var vehicleLanes = hwInfo.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .ToArray();

            const float laneWidth = 4f;
            var nbLanes = vehicleLanes.Count();
            var positionStart = laneWidth * ((1f - nbLanes) / 2f);

            for (var i = 0; i < nbLanes; i++)
            {
                var l = vehicleLanes[i];
                l.m_stopType = VehicleInfo.VehicleType.None;
                l.m_speedLimit = 2f;
                l.m_verticalOffset = 0f;
                l.m_width = laneWidth;
                l.m_position = positionStart + i * laneWidth;
                l.m_laneProps = l.m_laneProps.Clone();

                foreach (var prop in l.m_laneProps.m_props)
                {
                    prop.m_position = new Vector3(0, 0, 0);
                }
                if (isTwoWay)
                {
                    if (l.m_position < 0.0f)
                    {
                        l.m_direction = NetInfo.Direction.Backward;
                    }
                    else
                    {
                        l.m_direction = NetInfo.Direction.Forward;
                    }
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

        private static NetLaneProps.Prop GetHighwayLight(this IEnumerable<NetLaneProps.Prop> props)
        {
            var streetLightPropInfo = Prefabs.Find<PropInfo>("New Street Light Highway", false);
            if (streetLightPropInfo == null)
            {
                return null;
            }

            return props.FirstOrDefault(prop => prop.m_prop == streetLightPropInfo);
        }

        public static void SetHighwayLights(ICollection<NetLaneProps.Prop> leftprops, ICollection<NetLaneProps.Prop> rightprops, NetInfoVersion version)
        {
            var rightStreetLightProp = rightprops.GetHighwayLight();
            if (rightStreetLightProp == null)
            {
                return;
            }

            NetLaneProps.Prop leftStreetLightProp = null;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    leftStreetLightProp = rightStreetLightProp.ShallowClone();
                    leftStreetLightProp.m_segmentOffset = 39;
                    leftStreetLightProp.m_angle = 180;
                    leftStreetLightProp.m_repeatDistance = 80;
                    leftStreetLightProp.m_position = new Vector3(-1.75f, 0, 0);
                    leftStreetLightProp.m_endFlagsForbidden = NetNode.Flags.TrafficLights;
                    rightStreetLightProp.m_repeatDistance = 80;
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    leftStreetLightProp = rightStreetLightProp.ShallowClone();
                    leftStreetLightProp.m_segmentOffset = 39;
                    leftStreetLightProp.m_angle = 180;
                    leftStreetLightProp.m_repeatDistance = 80;
                    leftStreetLightProp.m_position = new Vector3(-1.75f, -2, 0);
                    leftStreetLightProp.m_endFlagsForbidden = NetNode.Flags.TrafficLights;
                    rightStreetLightProp.m_repeatDistance = 80;
                    rightStreetLightProp.m_position = new Vector3(1.75f, -2, 0);
                    rightStreetLightProp.m_endFlagsForbidden = NetNode.Flags.TrafficLights;
                    break;

                case NetInfoVersion.Tunnel:
                    leftStreetLightProp = rightStreetLightProp.ShallowClone();
                    leftStreetLightProp.m_angle = 180;
                    leftStreetLightProp.m_repeatDistance = 40;
                    leftStreetLightProp.m_segmentOffset = 0;
                    leftStreetLightProp.m_position = new Vector3(-3.2f, -4.5f, 0);
                    leftStreetLightProp.m_endFlagsForbidden = NetNode.Flags.TrafficLights;
                    rightStreetLightProp.m_repeatDistance = 40;
                    rightStreetLightProp.m_segmentOffset = 0;
                    rightStreetLightProp.m_position = new Vector3(3.2f, -4.5f, 0);
                    rightStreetLightProp.m_endFlagsForbidden = NetNode.Flags.TrafficLights;
                    break;

                case NetInfoVersion.Slope:
                    rightprops.Trim(p => p == rightStreetLightProp);
                    break;
            }

            if (leftStreetLightProp != null)
            {
                leftprops.Add(leftStreetLightProp);
            }
        }

        public static void SetHighwayRightLights(this ICollection<NetLaneProps.Prop> props, NetInfoVersion version)
        {
            var streetLightProp = props.GetHighwayLight();
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
                    streetLightProp.m_endFlagsForbidden = NetNode.Flags.TrafficLights;
                    break;

                case NetInfoVersion.Tunnel:
                    streetLightProp.m_repeatDistance = 40;
                    streetLightProp.m_segmentOffset = 0;
                    streetLightProp.m_position = new Vector3(3.2f, -4.5f, 0);
                    streetLightProp.m_endFlagsForbidden = NetNode.Flags.TrafficLights;
                    break;

                case NetInfoVersion.Slope:
                    props.Trim(p => p == streetLightProp);
                    break;
            }
        }

        public static void AddLeftWallLights(this ICollection<NetLaneProps.Prop> props, int xPos = 0)
        {
            var wallLightPropInfo = Prefabs.Find<PropInfo>("Wall Light Orange");
            var wallLightProp = new NetLaneProps.Prop
            {
                m_prop = wallLightPropInfo.ShallowClone(),
                m_probability = 100,
                m_repeatDistance = 20,
                m_segmentOffset = 0,
                m_angle = 270,
                m_position = new Vector3(xPos, 1.5f, 0)
            };
            props.Add(wallLightProp);
        }

        public static void AddRightWallLights(this ICollection<NetLaneProps.Prop> props, int xPos = 0)
        {
            var wallLightPropInfo = Prefabs.Find<PropInfo>("Wall Light Orange");
            var wallLightProp = new NetLaneProps.Prop
            {
                m_prop = wallLightPropInfo.ShallowClone(),
                m_probability = 100,
                m_repeatDistance = 20,
                m_segmentOffset = 0,
                m_angle = 90,
                m_position = new Vector3(xPos, 1.5f, 0)
            };
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

        public static void TrimNonHighwayProps(this NetInfo info, bool removeRightStreetLights = false, bool removeLeftStreetLights = true, bool removeMotorwaySigns = false)
        {
            var randomProp = Prefabs.Find<PropInfo>("Random Street Prop", false);
            var streetLight = Prefabs.Find<PropInfo>("New Street Light", false);
            var streetLightHw = Prefabs.Find<PropInfo>("New Street Light Highway", false);
            var manhole = Prefabs.Find<PropInfo>("Manhole", false);
            var motorwaySign = Prefabs.Find<PropInfo>("Motorway Sign", false);
            var motorwayOverroadSigns = Prefabs.Find<PropInfo>("Motorway Overroad Signs", false);

            foreach (var laneProps in info.m_lanes.Select(l => l.m_laneProps).Where(lpi => lpi != null))
            {
                var remainingProp = new List<NetLaneProps.Prop>();

                foreach (var prop in laneProps.m_props.Where(p => p.m_prop != null))
                {
                    var newProp = prop.ShallowClone();
                    if (prop.m_prop == randomProp)
                    {
                        continue;
                    }

                    if (prop.m_prop == manhole)
                    {
                        continue;
                    }

                    if (removeLeftStreetLights)
                    {
                        if (prop.m_prop == streetLight &&
                            laneProps.name.Contains("Left"))
                        {
                            prop.m_probability = 0;
                            //continue;
                        }

                        if (prop.m_prop == streetLightHw &&
                            laneProps.name.Contains("Left"))
                        {
                            prop.m_probability = 0;
                            //continue;
                        }
                    }

                    if (removeRightStreetLights)
                    {
                        if (prop.m_prop == streetLight &&
                            laneProps.name.Contains("Right"))
                        {
                            prop.m_probability = 0;
                            //continue;
                        }

                        if (prop.m_prop == streetLightHw &&
                            laneProps.name.Contains("Right"))
                        {
                            prop.m_probability = 0;
                            //continue;
                        }
                    }

                    if (removeMotorwaySigns)
                    {
                        if (prop.m_prop == motorwaySign || prop.m_prop == motorwayOverroadSigns)
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
