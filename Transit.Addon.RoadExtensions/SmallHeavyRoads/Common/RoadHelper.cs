using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.SmallHeavyRoads.Common
{
    public static class RoadHelper
    {
        public static NetInfo.Lane GetLeftRoadShoulder(this NetInfo info, NetInfo templateInfo, NetInfoVersion version)
        {
            var leftPedLane = info.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
            var templateLane = templateInfo.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
            var propOffset = -0.5f * (info.m_pavementWidth - 4);
            leftPedLane.m_laneProps = templateLane.m_laneProps.Clone("Left Road Shoulder Props");

            foreach (var prop in leftPedLane.m_laneProps.m_props)
            {
                if (version == NetInfoVersion.Tunnel)
                {
                    if (prop.m_prop.name.ToLower().Contains("street light"))
                    {
                        prop.m_position = new Vector3(-2.2f, -4.5f, 0);
                    }
                    if (prop.m_prop.name.ToLower().Contains("traffic light"))
                    {
                        prop.m_position = new Vector3(propOffset, -1.6f, 0);
                    }
                }
                else if (version == NetInfoVersion.Slope)
                {
                    if (prop.m_prop.name.ToLower().Contains("traffic light"))
                    {
                        prop.m_position = new Vector3(propOffset, -1.6f, 0);
                    }
                }
            }
            return leftPedLane;
        }

        public static NetInfo.Lane GetRightRoadShoulder(this NetInfo info, NetInfo templateInfo, NetInfoVersion version)
        {
            var rightPedLane = info.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
            var templateLane = templateInfo.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
            var propOffset = 0.5f * (info.m_pavementWidth - 4);

            rightPedLane.m_laneProps = templateLane.m_laneProps.Clone("Right Road Shoulder Props");

            foreach (var prop in rightPedLane.m_laneProps.m_props)
            {
                if (version == NetInfoVersion.Tunnel)
                {
                    if (prop.m_prop.name.ToLower().Contains("street light"))
                    {
                        prop.m_position = new Vector3(2.2f, -4.5f, 0);
                    }

                    if (prop.m_prop.name.ToLower().Contains("traffic light"))
                    {
                        prop.m_position = new Vector3(propOffset, -1.6f, 0);
                    }
                }
                else if (version == NetInfoVersion.Slope)
                {
                    if (prop.m_prop.name.ToLower().Contains("traffic light"))
                    {
                        prop.m_position = new Vector3(propOffset, -1.6f, 0);
                    }
                }
            }

            return rightPedLane;
        }

        public static NetLaneProps GetLeftRoadProps(this NetInfo info, NetInfo templateInfo)
        {
            var laneProps = info.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.Pedestrian).m_laneProps.Clone("left road props");
            if (laneProps == null)
            {
                laneProps = templateInfo.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.Pedestrian).m_laneProps.Clone("left road props");
            }
            return laneProps;
        }

        public static NetLaneProps GetRightRoadProps(this NetInfo info, NetInfo templateInfo)
        {
            var laneProps = info.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.Pedestrian).m_laneProps.Clone("right road props");
            if (laneProps == null)
            {
                laneProps = templateInfo.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.Pedestrian).m_laneProps.Clone("right road props");
            }
            return laneProps;
        }

        public static void AddLeftWallLights(this ICollection<NetLaneProps.Prop> props, float pavementWidth)
        {
            var wallLightPropInfo = Prefabs.Find<PropInfo>("Wall Light Orange");
            var wallLightProp = new NetLaneProps.Prop();
            var wallPropXPos = (pavementWidth - 3) * -0.5f;
            wallLightProp.m_prop = wallLightPropInfo.ShallowClone();
            wallLightProp.m_probability = 100;
            wallLightProp.m_repeatDistance = 20;
            wallLightProp.m_segmentOffset = 0;
            wallLightProp.m_angle = 270;
            wallLightProp.m_position = new Vector3(wallPropXPos, 1.5f, 0);
            props.Add(wallLightProp);
        }

        public static void AddRightWallLights(this ICollection<NetLaneProps.Prop> props, float pavementWidth)
        {
            var wallLightPropInfo = Prefabs.Find<PropInfo>("Wall Light Orange");
            var wallLightProp = new NetLaneProps.Prop();
            var wallPropXPos = (pavementWidth - 3) * 0.5f;
            wallLightProp.m_prop = wallLightPropInfo.ShallowClone();
            wallLightProp.m_probability = 100;
            wallLightProp.m_repeatDistance = 20;
            wallLightProp.m_segmentOffset = 0;
            wallLightProp.m_angle = 90;
            wallLightProp.m_position = new Vector3(wallPropXPos, 1.5f, 0);
            props.Add(wallLightProp);
        }

        public static void TrimAboveGroundProps(this NetInfo info, NetInfoVersion version, bool removeRightStreetLights = false, bool removeLeftStreetLights = false)
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
                    if ((version == NetInfoVersion.Tunnel || version == NetInfoVersion.Slope)
                     && (prop.m_prop.name.ToLower().Contains("random")
                        || prop.m_prop.name.ToLower().Contains("manhole")
                        || prop.m_prop.name.ToLower().Contains("street name sign")))
                    {
                        continue;
                    }
                    if ((version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
                        && (prop.m_prop.name.ToLower().Contains("random")
                            || prop.m_prop.name.ToLower().Contains("manhole")))
                    {
                        continue;
                    }
                    if (version == NetInfoVersion.Slope && prop.m_prop.name.ToLower().Contains("street light"))
                    {
                        continue;
                    }
                    //if (prop.m_prop == manhole)
                    //{
                    //    continue;
                    //}

                    //if (removeLeftStreetLights)
                    //{
                    //    if (prop.m_prop == streetLight &&
                    //        laneProps.name.Contains("Left"))
                    //    {
                    //        continue;
                    //    }

                    //    if (prop.m_prop == streetLightHw &&
                    //        laneProps.name.Contains("Left"))
                    //    {
                    //        continue;
                    //    }
                    //}

                    //if (removeRightStreetLights)
                    //{
                    //    if (prop.m_prop == streetLight &&
                    //        laneProps.name.Contains("Right"))
                    //    {
                    //        continue;
                    //    }

                    //    if (prop.m_prop == streetLightHw &&
                    //        laneProps.name.Contains("Right"))
                    //    {
                    //        continue;
                    //    }
                    //}

                    remainingProp.Add(prop);
                }

                laneProps.m_props = remainingProp.ToArray();
            }
        }

        public static NetInfo SetRoadLanes(this NetInfo rdInfo, NetInfoVersion version, int lanesToAdd = 0, float pedPropOffsetX = 0.0f, float speedLimit = -1, bool isTwoWay = false, bool hasCenterTurningLane = false)
        {
            if (lanesToAdd < 0)
            {
                var remainingLanes = new List<NetInfo.Lane>();
                remainingLanes.AddRange(rdInfo
                    .m_lanes
                    .Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian || l.m_laneType == NetInfo.LaneType.None || l.m_laneType == NetInfo.LaneType.Parking));
                remainingLanes.AddRange(rdInfo
                    .m_lanes
                    .Where(l => l.m_laneType != NetInfo.LaneType.None)
                    .Skip(-lanesToAdd));

                rdInfo.m_lanes = remainingLanes.ToArray();
            }
            else if (lanesToAdd > 0)
            {
                var sourceLane = rdInfo.m_lanes.First(l => l.m_laneType != NetInfo.LaneType.None && l.m_laneType != NetInfo.LaneType.Parking && l.m_laneType != NetInfo.LaneType.Pedestrian);
                var tempLanes = rdInfo.m_lanes.ToList();

                for (var i = 0; i < lanesToAdd; i++)
                {
                    var newLane = sourceLane.Clone();
                    tempLanes.Add(newLane);
                }

                rdInfo.m_lanes = tempLanes.ToArray();
            }

            var vehicleLanes = rdInfo.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None && l.m_laneType != NetInfo.LaneType.Parking && l.m_laneType != NetInfo.LaneType.Pedestrian)
                .ToArray();

            const float laneWidth = 3f;
            var nbLanes = vehicleLanes.Count();
            var positionStart = laneWidth * (((hasCenterTurningLane ? 2 : 1) - nbLanes  ) / 2f);

            for (var i = 0; i < nbLanes; i++)
            {
                var l = vehicleLanes[i];
                l.m_position = positionStart + i * laneWidth;
                var isTurningLane = (hasCenterTurningLane && (i == nbLanes - 1 || l.m_position == 0));
                if (isTurningLane)
                {
                    l.m_position = 0;
                }
                l.m_allowStop = false;
                l.m_width = laneWidth;

                l.m_laneProps = l.m_laneProps.Clone();
                if (speedLimit > -1 && !isTurningLane)
                {
                    l.m_speedLimit = speedLimit;
                }
                else if (isTurningLane)
                {
                    l.m_speedLimit = 0.6f;
                    l.m_allowConnect = false;
                    SetupTurningLaneProps(l);
                }

                if (isTwoWay)
                {
                    if ((!isTurningLane && l.m_position < 0.0f) || (isTurningLane && i == nbLanes - 1))
                    {
                        l.m_direction = NetInfo.Direction.Backward;
                        l.m_finalDirection = NetInfo.Direction.Backward;
                    }
                    else
                    {
                        l.m_direction = NetInfo.Direction.Forward;
                        l.m_finalDirection = NetInfo.Direction.Forward;
                    }
                }

                foreach (var prop in l.m_laneProps.m_props)
                {
                    prop.m_position = new Vector3(0, 0, 0);
                }
            }

            var laneCollection = new List<NetInfo.Lane>();

            laneCollection.AddRange(vehicleLanes);
            laneCollection.AddRange(rdInfo.SetPedestrianLanes(version, pedPropOffsetX));

            if (rdInfo.m_hasParkingSpaces)
            {
                laneCollection.AddRange(rdInfo.SetParkingLanes());
            }

            rdInfo.m_lanes = laneCollection.OrderBy(lc=>lc.m_position).ToArray();

            return rdInfo;
        }

        private static IEnumerable<NetInfo.Lane> SetPedestrianLanes(this NetInfo rdInfo, NetInfoVersion version, float propOffsetX = 0.0f)
        {
            var pedestrianLanes = rdInfo.m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian)
                .OrderBy(l => l.m_position)
                .ToArray();

            if (pedestrianLanes.Length > 0)
            {
                foreach (var pedLane in pedestrianLanes)
                {
                    var multiplier = pedLane.m_position / Math.Abs(pedLane.m_position);
                    pedLane.m_width = rdInfo.m_pavementWidth - (version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel ? 3 : 1);
                    pedLane.m_position = multiplier * (rdInfo.m_halfWidth - ((version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel ? 2 : 0) + 0.5f * pedLane.m_width));
                    if (propOffsetX != 0.0f && pedLane.m_laneProps != null)
                    {
                        foreach (var pedLaneProp in pedLane.m_laneProps.m_props)
                        {
                            pedLaneProp.m_position.x += propOffsetX * multiplier;
                        }
                    }
                }
            }
            return pedestrianLanes;
        }

        private static IEnumerable<NetInfo.Lane> SetParkingLanes(this NetInfo rdInfo)
        {
            var parkingLanes = rdInfo
                .m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Parking)
                .ToArray();

            return parkingLanes;
        }
        private static void SetupTurningLaneProps(NetInfo.Lane lane)
        {
            var isLeftDriving = Singleton<SimulationManager>.instance.m_metaData.m_invertTraffic == SimulationMetaData.MetaBool.True;

            if (lane.m_laneProps == null)
            {
                return;
            }

            if (lane.m_laneProps.m_props == null)
            {
                return;
            }

            var fwd = lane.m_laneProps.m_props.FirstOrDefault(p => p.m_flagsRequired == NetLane.Flags.Forward);
            var left = lane.m_laneProps.m_props.FirstOrDefault(p => p.m_flagsRequired == NetLane.Flags.Left);
            var right = lane.m_laneProps.m_props.FirstOrDefault(p => p.m_flagsRequired == NetLane.Flags.Right);

            if (fwd == null)
            {
                return;
            }

            if (left == null)
            {
                return;
            }

            if (right == null)
            {
                return;
            }


            // Existing props
            //var r0 = NetLane.Flags.Forward; 
            //var r1 = NetLane.Flags.ForwardRight;
            //var r2 = NetLane.Flags.Left;
            //var r3 = NetLane.Flags.LeftForward;
            //var r4 = NetLane.Flags.LeftForwardRight;
            //var r5 = NetLane.Flags.LeftRight;
            //var r6 = NetLane.Flags.Right;

            //var f0 = NetLane.Flags.LeftRight;
            //var f1 = NetLane.Flags.Left;
            //var f2 = NetLane.Flags.ForwardRight;
            //var f3 = NetLane.Flags.Right;
            //var f4 = NetLane.Flags.None;
            //var f5 = NetLane.Flags.Forward;
            //var f6 = NetLane.Flags.LeftForward;


            var newProps = new FastList<NetLaneProps.Prop>();

            //newProps.Add(fwd); // Do we want "Forward" on a turning lane?
            newProps.Add(left);
            newProps.Add(right);

            var fl = left.ShallowClone();
            fl.m_flagsRequired = NetLane.Flags.LeftForward;
            fl.m_flagsForbidden = NetLane.Flags.Right;
            newProps.Add(fl);

            var fr = right.ShallowClone();
            fr.m_flagsRequired = NetLane.Flags.ForwardRight;
            fr.m_flagsForbidden = NetLane.Flags.Left;
            newProps.Add(fr);

            var flr = isLeftDriving ? right.ShallowClone() : left.ShallowClone();
            flr.m_flagsRequired = NetLane.Flags.LeftForwardRight;
            flr.m_flagsForbidden = NetLane.Flags.None;
            newProps.Add(flr);

            var lr = isLeftDriving ? right.ShallowClone() : left.ShallowClone();
            lr.m_flagsRequired = NetLane.Flags.LeftRight;
            lr.m_flagsForbidden = NetLane.Flags.Forward;
            newProps.Add(lr);

            lane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
            lane.m_laneProps.name = "TurningLane";
            lane.m_laneProps.m_props = newProps.ToArray();
        }
    }
}
