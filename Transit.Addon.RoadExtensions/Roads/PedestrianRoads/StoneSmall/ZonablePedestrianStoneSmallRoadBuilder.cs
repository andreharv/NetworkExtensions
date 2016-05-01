using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.StoneSmall
{
    public partial class ZonablePedestrianStoneSmallRoadBuilder : Activable, INetInfoBuilderPart, INetInfoLateBuilder
    {
        public int Order { get { return 320; } }
        public int UIOrder { get { return 40; } }

        public const string NAME = "Zonable Pedestrian Small Stone Road";
        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string Name { get { return NAME; } }
        public string DisplayName { get { return "[BETA] Zonable Pedestrian Small Stone Road"; } }
        public string Description { get { return "Small stone pedestrian Roads are only accessible to pedestrians and emergency vehicles"; } }
        public string ShortDescription { get { return "No Passenger Vehicles, zoneable [Traffic++ V2 required]"; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_PEDESTRIANS; } }

        public string ThumbnailsPath { get { return @"Roads\PedestrianRoads\StoneSmall\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\PedestrianRoads\StoneSmall\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L_TREES);
            var roadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L_TUNNEL);

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup8mNoSWMesh(version);


            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;

            info.m_surfaceLevel = 0;
            info.m_createPavement = true;
            info.m_createGravel = false;
            //info.m_averageVehicleLaneSpeed = 0.3f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = true;
            info.m_halfWidth = 4;
            info.m_UnlockMilestone = roadInfo.m_UnlockMilestone;
            info.m_pavementWidth = 2;
            info.m_dlcRequired = SteamHelper.DLC_BitMask.AfterDarkDLC;

            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
                info.m_class = roadTunnelInfo.m_class.Clone("NExtPedRoad8mTunnel");
            }
            else
            {
                info.m_class = roadInfo.m_class.Clone("NExtPedRoad8m");
            }

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
            var sVehicleLaneWidth = 2.5f;
            var sVehicleLanePosAbs = 2f;
            //var tempVLanes = new List<NetInfo.Lane>();
            for (int i = 0; i < vehicleLanes.Count; i++)
            {
                vehicleLanes[i].m_verticalOffset = 0f;
                vehicleLanes[i] = new ExtendedNetInfoLane(vehicleLanes[i], ExtendedVehicleType.ServiceVehicles | ExtendedVehicleType.CargoTruck | ExtendedVehicleType.SnowTruck)
                {
                    m_position = (Math.Abs(vehicleLanes[i].m_position) / vehicleLanes[i].m_position) * sVehicleLanePosAbs,
                    m_width = sVehicleLaneWidth,
                    m_verticalOffset = 0.05f
                };
            }
            var pedLane = new NetInfo.Lane();
            pedLane = info.m_lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
            pedLane.m_position = 0;
            pedLane.m_width = 8;
            pedLane.m_verticalOffset = 0.05f;
            var tempProps = new List<NetLaneProps.Prop>();
            tempProps = pedLane.m_laneProps.m_props.ToList();
            tempProps.RemoveProps(new string[] { "random", "bus", "limit" });
            tempProps.ReplacePropInfo(new KeyValuePair<string, PropInfo>("street light", Prefabs.Find<PropInfo>("StreetLamp02")));
            tempProps.ReplacePropInfo(new KeyValuePair<string, PropInfo>("traffic light 01", Prefabs.Find<PropInfo>("Traffic Light 01 Mirror")));
            var pedLightProp = tempProps.FirstOrDefault(tp => tp.m_prop.name == "Traffic Light 01 Mirror").ShallowClone();
            var pedLightPropInfo = Prefabs.Find<PropInfo>("Traffic Light Pedestrian");
            pedLightProp.m_prop = pedLightPropInfo;
            pedLightProp.m_position.x = -3.5f;
            var lights = tempProps.Where(tp => tp.m_prop.name == "StreetLamp02").ToList();
            foreach (var light in lights)
            {
                light.m_position.x = 0;
            }
            var tLight = tempProps.LastOrDefault(tp => tp.m_prop.name == "Traffic Light 02");
            tLight.m_position.x = -3.5f;
            var pedLightProp2 = tempProps.FirstOrDefault(tp => tp.m_prop.name == "Traffic Light 02").ShallowClone();
            pedLightProp2.m_prop = pedLightPropInfo;
            pedLightProp2.m_position.x = 3.5f;
            tempProps.ReplacePropInfo(new KeyValuePair<string, PropInfo>("traffic light 02", Prefabs.Find<PropInfo>("Traffic Light 01 Mirror")));
            tempProps.Add(pedLightProp);
            tempProps.Add(pedLightProp2);
            pedLane.m_laneProps.m_props = tempProps.ToArray();

            var roadCollection = new List<NetInfo.Lane>();
            roadCollection.Add(pedLane);
            roadCollection.AddRange(vehicleLanes);
            info.m_lanes = roadCollection.ToArray();
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

        public void LateBuildUp(NetInfo info, NetInfoVersion version)
        {
            if (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated)
            {
                var pillar = Prefabs.Find<BuildingInfo>("Pedestrian Elevated Pillar", false);
                var bridgeAI = info.GetComponent<RoadBridgeAI>();
                if (bridgeAI != null)
                {
                    bridgeAI.m_doubleLength = false;
                    bridgeAI.m_bridgePillarInfo = pillar;
                    bridgeAI.m_middlePillarInfo = null;
                    bridgeAI.m_bridgePillarOffset = 0;
                }
            }

            var pedLane = new NetInfo.Lane();
            pedLane = info.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ShallowClone();
            var stoneBollard = PrefabCollection<PropInfo>.FindLoaded("478820060.StoneBollard_Data");
            if (stoneBollard == null)
            {
                stoneBollard = PrefabCollection<PropInfo>.FindLoaded("StoneBollard.StoneBollard_Data");
            }

            var bollardProp = new NetLaneProps.Prop()
            {
                m_prop = stoneBollard,
                m_finalProp = stoneBollard,
                m_probability = 100,
                m_segmentOffset = 1,
                m_minLength = 10,
                m_endFlagsRequired = NetNode.Flags.Transition
            };
            bollardProp.m_position.x = -3.5f;
            bollardProp.m_position.y = -0.3f;

            var bollardProp2 = bollardProp.ShallowClone();
            bollardProp2.m_segmentOffset = -1;
            bollardProp2.m_endFlagsRequired = NetNode.Flags.None;
            bollardProp2.m_startFlagsRequired = NetNode.Flags.Transition;

            var bollardProp3 = bollardProp.ShallowClone();
            bollardProp3.m_position.x = 3.5f;

            var bollardProp4 = bollardProp2.ShallowClone();
            bollardProp4.m_position.x = 3.5f;

            var bollardProp5 = bollardProp.ShallowClone();
            bollardProp5.m_position.x = 0;

            var bollardProp6 = bollardProp2.ShallowClone();
            bollardProp6.m_position.x = 0;

            var tempProps = pedLane.m_laneProps.m_props.ToList();
            tempProps.AddRange(new List<NetLaneProps.Prop> { bollardProp, bollardProp2, bollardProp3, bollardProp4, bollardProp5, bollardProp6 });
            pedLane.m_laneProps.m_props = tempProps.ToArray();

            //var pedLaneCollection = new List<NetInfo.Lane>();
            //pedLaneCollection.Add(pedLane);
            //pedLaneCollection.AddRange(info.m_lanes.Where(l => l.m_laneType != NetInfo.LaneType.Pedestrian));
            //info.m_lanes = pedLaneCollection.ToArray();
        }
    }
}
