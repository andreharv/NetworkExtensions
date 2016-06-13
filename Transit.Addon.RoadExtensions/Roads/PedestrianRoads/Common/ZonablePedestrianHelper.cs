using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Network;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Common
{
    public static partial class ZonablePedestrianHelper
    {
        public static void SetupTinyPed(this NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L_TREES);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            info.m_surfaceLevel = 0;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = true;
            info.m_halfWidth = 4;
            info.m_UnlockMilestone = roadInfo.m_UnlockMilestone;
            info.m_pavementWidth = 2;
            info.m_class = roadInfo.m_class.Clone($"NExt {info.name}");
            info.m_class.m_level = ItemClass.Level.Level5;
            // Setting up lanes
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                LaneWidth = 2.5f,
                SpeedLimit = 0.3f,
                HasBusStop = false,
                PedPropOffsetX = -2
            });

            var vehicleLanes = new List<NetInfo.Lane>();
            vehicleLanes.AddRange(info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).ToList());

            for (var i = 0; i < vehicleLanes.Count; i++)
            {
                vehicleLanes[i].m_verticalOffset = 0f;
                vehicleLanes[i] = new ExtendedNetInfoLane(vehicleLanes[i], ExtendedVehicleType.ServiceVehicles | ExtendedVehicleType.CargoTruck | ExtendedVehicleType.SnowTruck)
                {
                    m_position = (Math.Abs(vehicleLanes[i].m_position) / vehicleLanes[i].m_position) * 2f,
                    m_width = 2.5f,
                    m_verticalOffset = 0.05f
                };
            }

            foreach (var lane in vehicleLanes)
            {
                var props = lane.m_laneProps.m_props.ToList();
                props.RemoveProps("arrow", "manhole");
                lane.m_laneProps.m_props = props.ToArray();
            }

            var pedLane = info.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
            pedLane.m_position = 0;
            pedLane.m_width = 8;
            pedLane.m_verticalOffset = 0.05f;

            var pedLaneProps = pedLane.m_laneProps.m_props.ToList();
            pedLaneProps.RemoveProps("random", "bus", "limit");
            pedLaneProps.ReplacePropInfo(new KeyValuePair<string, PropInfo>("street light", Prefabs.Find<PropInfo>("StreetLamp02")));
            pedLaneProps.ReplacePropInfo(new KeyValuePair<string, PropInfo>("traffic light 01", Prefabs.Find<PropInfo>("Traffic Light 01 Mirror")));
            var pedLightProp = pedLaneProps.First(tp => tp.m_prop.name == "Traffic Light 01 Mirror").ShallowClone();
            var pedLightPropInfo = Prefabs.Find<PropInfo>("Traffic Light Pedestrian");
            pedLightProp.m_prop = pedLightPropInfo;
            pedLightProp.m_position.x = -3.5f;
            
            foreach (var light in pedLaneProps.Where(tp => tp.m_prop.name == "StreetLamp02"))
            {
                light.m_position.x = 0;
            }

            var tLight = pedLaneProps.LastOrDefault(tp => tp.m_prop.name == "Traffic Light 02");
            if (tLight != null)
            {
                tLight.m_position.x = -3.5f;
            }

            var pedLightProp2 = pedLaneProps.First(tp => tp.m_prop.name == "Traffic Light 02").ShallowClone();
            pedLightProp2.m_prop = pedLightPropInfo;
            pedLightProp2.m_position.x = 3.5f;
            pedLaneProps.ReplacePropInfo(new KeyValuePair<string, PropInfo>("traffic light 02", Prefabs.Find<PropInfo>("Traffic Light 01 Mirror")));
            pedLaneProps.Add(pedLightProp);
            pedLaneProps.Add(pedLightProp2);
            pedLane.m_laneProps.m_props = pedLaneProps.ToArray();

            var laneCollection = new List<NetInfo.Lane>();
            laneCollection.Add(pedLane);
            laneCollection.AddRange(vehicleLanes);
            info.m_lanes = laneCollection.ToArray();

            ///////////////////////////
            // AI                    //
            ///////////////////////////
            var hwPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (hwPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost;
                playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost;
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = false;
                roadBaseAI.m_noiseAccumulation = 3;
                roadBaseAI.m_noiseRadius = 20;
            }
        }

        // T++ Legacy
        //public static void SetupPed(this NetInfo info, NetInfoVersion version)
        //{
        //    ///////////////////////////
        //    // Templates             //
        //    ///////////////////////////
        //    var onewayRoad = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L);
        //    var pedestrianVanilla = Prefabs.Find<NetInfo>(NetInfos.Vanilla.PED_PAVEMENT);
        //    var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L_TREES);

        //    ///////////////////////////
        //    // Set up                //
        //    ///////////////////////////
        //    info.m_availableIn = ItemClass.Availability.All;
        //    info.m_surfaceLevel = 0;
        //    info.m_UnlockMilestone = onewayRoad.m_UnlockMilestone;
        //    info.m_class = roadInfo.m_class.Clone("NExtPedRoad");
        //    info.m_class.m_level = ItemClass.Level.Level5;
        //    info.m_halfWidth = 4;
        //    info.m_UnlockMilestone = roadInfo.m_UnlockMilestone;
        //    info.m_pavementWidth = 2;

        //    switch (version)
        //    {
        //        case NetInfoVersion.Ground:
        //            {
        //                info.m_lanes = info
        //                    .m_lanes
        //                    .Where(l => l.m_laneType != NetInfo.LaneType.Parking)
        //                    .OrderBy(l => l.m_position)
        //                    .ToArray();

        //                info.m_lanes[0].m_position = -4f;
        //                info.m_lanes[0].m_width = 2f;
        //                info.m_lanes[0].m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();

        //                if (pedestrianVanilla != null)
        //                {
        //                    info.m_lanes[0].m_laneProps.m_props = new NetLaneProps.Prop[1];
        //                    Array.Copy(pedestrianVanilla.m_lanes[0].m_laneProps.m_props, info.m_lanes[0].m_laneProps.m_props, 1);
        //                }
        //                else
        //                {
        //                    info.m_lanes[0].m_laneProps.m_props = new NetLaneProps.Prop[0];
        //                }
        //                info.m_lanes[0].m_laneProps.m_props[0].m_position.x = 0.0f;
        //                info.m_lanes[0].m_stopType = VehicleInfo.VehicleType.None;

        //                info.m_lanes[3].m_position = 4f;
        //                info.m_lanes[3].m_width = 2f;
        //                info.m_lanes[3].m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
        //                info.m_lanes[3].m_laneProps.m_props = new NetLaneProps.Prop[0];
        //                info.m_lanes[3].m_stopType = VehicleInfo.VehicleType.None;

        //                info.m_lanes[1] = new ExtendedNetInfoLane(info.m_lanes[1], ExtendedVehicleType.ServiceVehicles);
        //                info.m_lanes[1].m_position = -1.25f;
        //                info.m_lanes[1].m_speedLimit = 0.3f;
        //                info.m_lanes[1].m_stopType = VehicleInfo.VehicleType.None;
        //                info.m_lanes[1].m_laneType = NetInfo.LaneType.Vehicle;

        //                info.m_lanes[2] = new ExtendedNetInfoLane(info.m_lanes[2], ExtendedVehicleType.ServiceVehicles);
        //                info.m_lanes[2].m_position = 1.25f;
        //                info.m_lanes[2].m_speedLimit = 0.3f;
        //                info.m_lanes[2].m_stopType = VehicleInfo.VehicleType.None;
        //                info.m_lanes[2].m_laneType = NetInfo.LaneType.Vehicle;

        //                var centerLane = info.m_lanes[3].ShallowClone();
        //                centerLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
        //                centerLane.m_laneProps.m_props = new NetLaneProps.Prop[0];
        //                centerLane.m_position = 0f;

        //                info.m_lanes = info.m_lanes.Union(centerLane).ToArray();
        //            }
        //            break;

        //        case NetInfoVersion.Elevated:
        //        case NetInfoVersion.Bridge:
        //            {
        //                NetInfo.Lane[] lanes = new NetInfo.Lane[3];
        //                lanes[0] = info.m_lanes[0];
        //                lanes[0].m_stopType = VehicleInfo.VehicleType.None;
        //                lanes[0].m_laneProps = null;

        //                // Backward Lane
        //                lanes[1] = new ExtendedNetInfoLane(ExtendedVehicleType.ServiceVehicles);
        //                lanes[1].m_position = -1.5f;
        //                lanes[1].m_width = 2f;
        //                lanes[1].m_speedLimit = 0.3f;
        //                lanes[1].m_stopType = VehicleInfo.VehicleType.None;
        //                lanes[1].m_direction = NetInfo.Direction.Backward;
        //                lanes[1].m_laneType = NetInfo.LaneType.Vehicle;
        //                lanes[1].m_vehicleType = VehicleInfo.VehicleType.Car;

        //                // Forward Lane
        //                lanes[2] = new ExtendedNetInfoLane(ExtendedVehicleType.ServiceVehicles);
        //                lanes[2].m_position = 1.5f;
        //                lanes[2].m_width = 2f;
        //                lanes[2].m_speedLimit = 0.3f;
        //                lanes[2].m_stopType = VehicleInfo.VehicleType.None;
        //                lanes[2].m_laneType = NetInfo.LaneType.Vehicle;
        //                lanes[2].m_vehicleType = VehicleInfo.VehicleType.Car;

        //                info.m_lanes = lanes;
        //            }
        //            break;
        //    }
        //}
    }
}
