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
    }
}
