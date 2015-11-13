using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.Roads
{
    public static class RoadHelper
    {
        public static ICollection<NetLaneProps.Prop> GetLeftHWProps(this NetInfo rdInfo)
        {
            var leftProps = rdInfo.m_lanes.Where(l => l.m_laneProps != null && l.m_laneProps.name.ToLower().Contains("left")).Select(lp => (NetLaneProps.Prop)lp.m_laneProps.m_props.Clone()).ToList();
            return leftProps;
        }

        public static ICollection<NetLaneProps.Prop> GetRightHWProps(this NetInfo rdInfo)
        {
            var leftProps = rdInfo.m_lanes.Where(l => l.m_laneProps != null && l.m_laneProps.name.ToLower().Contains("left")).Select(lp => (NetLaneProps.Prop)lp.m_laneProps.m_props.Clone()).ToList();
            return leftProps;
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

        public static NetInfo SetRoadLanes(this NetInfo rdInfo, NetInfoVersion version, int lanesToAdd = 0)
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
            var positionStart = laneWidth * ((1f - nbLanes) / 2f);

            for (var i = 0; i < vehicleLanes.Length; i++)
            {
                var l = vehicleLanes[i];
                l.m_allowStop = false;
                l.m_width = laneWidth;
                l.m_position = positionStart + i * laneWidth;
                l.m_laneProps = l.m_laneProps.Clone();

                foreach (var prop in l.m_laneProps.m_props)
                {
                    prop.m_position = new Vector3(0, 0, 0);
                }
            }

            var laneCollection = new List<NetInfo.Lane>();

            laneCollection.AddRange(vehicleLanes);
            laneCollection.AddRange(rdInfo.SetPedestrianLanes(version));

            if (rdInfo.m_hasParkingSpaces)
            {
                laneCollection.AddRange(rdInfo.SetParkingLanes());
            }

            rdInfo.m_lanes = laneCollection.ToArray();

            return rdInfo;
        }

        private static IEnumerable<NetInfo.Lane> SetPedestrianLanes(this NetInfo rdInfo, NetInfoVersion version)
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
    }
}
