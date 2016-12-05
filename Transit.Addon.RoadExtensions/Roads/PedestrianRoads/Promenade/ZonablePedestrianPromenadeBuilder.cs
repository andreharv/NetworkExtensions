using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Promenade
{
    public partial class ZonablePedestrianPromenadeBuilder : Activable, INetInfoBuilderPart, INetInfoLateBuilder, IDLCRequired
    {
        public SteamHelper.DLC RequiredDLC { get { return SteamHelper.DLC.AfterDarkDLC; } }

        public int Order { get { return 330; } }
        public int UIOrder { get { return 30; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string Name { get { return "Zonable Promenade"; } }
        public string DisplayName { get { return "Zonable Promenade"; } }
        public string Description { get { return "Promenade is only accessible to pedestrians, cyclists, and emergency vehicles"; } }
        public string ShortDescription { get { return "Zoneable, No Passenger Vehicles [AfterDark DLC AND Traffic++ V2 required]"; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_PEDESTRIANS; } }

        public string ThumbnailsPath { get { return @"Roads\PedestrianRoads\Promenade\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\PedestrianRoads\Promenade\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground | NetInfoVersion.Elevated; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L_TREES);
            var roadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L_TUNNEL);
            info.m_connectGroup = (NetInfo.ConnectGroup)16;
            info.m_requireDirectRenderers = true;
            info.m_nodeConnectGroups = (NetInfo.ConnectGroup)16;

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup16mNoSWMesh(version);

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
            info.m_canCrossLanes = false;
            //info.m_averageVehicleLaneSpeed = 0.3f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = true;
            info.m_halfWidth = 8;
            info.m_UnlockMilestone = roadInfo.m_UnlockMilestone;
            info.m_pavementWidth = 2;
            info.m_requireSurfaceMaps = true;
            info.m_dlcRequired = SteamHelper.DLC_BitMask.AfterDarkDLC;
            var pedModdedLanes = info.SetRoadLanes(version, new LanesConfiguration() { PedPropOffsetX = 3.5f, LanesToAdd = 2, SpeedLimit = 0.2f });

            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
                info.m_class = roadTunnelInfo.m_class.Clone("NExtPedRoad16m");
            }
            else
            {
                info.m_class = roadInfo.m_class.Clone("NExtPedRoad16m");
            }
            info.m_class.m_level = ItemClass.Level.Level5;

            // Setting up lanes
            var vehicleLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).ToList();
            var bikeLaneWidth = 2;
            var bikeLanePosAbs = 1;
            var sVehicleLaneWidth = 2.5f;
            var sVehicleLanePosAbs = 4f;

            var bikeLanes = new List<NetInfo.Lane>();
            bikeLanes.AddRange(vehicleLanes.Take(2));

            var carLanes = new List<NetInfo.Lane>();
            carLanes.AddRange(vehicleLanes.Skip(2));
            
            for (var i = 0; i < bikeLanes.Count; i++)
            {
                bikeLanes[i].m_vehicleType = VehicleInfo.VehicleType.Bicycle;
                bikeLanes[i].m_position = ((i * 2) - 1) * bikeLanePosAbs;
                bikeLanes[i].m_width = bikeLaneWidth;
                bikeLanes[i].m_verticalOffset = -0.15f;
                bikeLanes[i].m_direction = bikeLanes[i].m_position > 0 ? NetInfo.Direction.Forward : NetInfo.Direction.Backward;
                bikeLanes[i].m_speedLimit = 0.8f;
                bikeLanes[i].m_stopType = VehicleInfo.VehicleType.None;
                var tempProps = bikeLanes[i].m_laneProps.m_props.ToList();
                tempProps.RemoveProps("arrow");
                bikeLanes[i].m_laneProps.m_props = tempProps.ToArray();
            }
            
            for (int i = 0; i < carLanes.Count; i++)
            {
                carLanes[i].m_verticalOffset = 0.05f;
                var position = ((i * 2) - 1) * sVehicleLanePosAbs;
                var niLane = new ExtendedNetInfoLane(carLanes[i], ExtendedVehicleType.ServiceVehicles | ExtendedVehicleType.CargoTruck | ExtendedVehicleType.SnowTruck)
                {
                    m_position = position,
                    m_width = sVehicleLaneWidth,
                    m_verticalOffset = 0.05f,
                    m_direction = position > 0 ? NetInfo.Direction.Forward : NetInfo.Direction.Backward
                };
                carLanes[i] = niLane;
                var tempProps = carLanes[i].m_laneProps.m_props.ToList();
                tempProps.RemoveProps("arrow", "limit");
                carLanes[i].m_laneProps.m_props = tempProps.ToArray();

            }
            var pedLanes = new List<NetInfo.Lane>();
            pedLanes.AddRange(info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).OrderBy(l => l.m_position));
            
            foreach (var lane in vehicleLanes)
            {
                var laneProps = lane.m_laneProps.m_props.ToList();
                laneProps.RemoveProps("arrow", "manhole");
                lane.m_laneProps.m_props = laneProps.ToArray();
            }

            for (int i = 0; i < pedLanes.Count; i++)
            {
                pedLanes[i].m_position = ((i * 2) - 1) * 5;
                pedLanes[i].m_width = 6;
                var tempProps = pedLanes[i].m_laneProps.m_props.ToList();
                tempProps.RemoveProps("bus", "random", "limit");
                var tempPropProps = tempProps.Where(tp => tp.m_prop != null);
                if (tempPropProps.Any(tp => tp.m_prop.name.ToLower().IndexOf("street light", StringComparison.Ordinal) != -1))
                {
                    tempProps.ReplacePropInfo(new KeyValuePair<string, PropInfo>("street light", Prefabs.Find<PropInfo>("StreetLamp02")));
                    var lightProp = tempProps.First(tp => tp.m_prop.name == "StreetLamp02");
                    lightProp.m_repeatDistance = 80;
                    lightProp.m_segmentOffset = i;
                    lightProp.m_probability = 100;
                    lightProp.m_position.x = ((i * 2) - 1) * -2.5f;
                }
                else
                {
                    var lightProp = new NetLaneProps.Prop()
                    {
                        m_prop = Prefabs.Find<PropInfo>("StreetLamp02").ShallowClone(),
                        m_repeatDistance = 80,
                        m_segmentOffset = i,
                        m_probability = 100
                    };
                    lightProp.m_position.x = ((i * 2) - 1) * -2.5f;
                    tempProps.Add(lightProp);
                }
                if (version == NetInfoVersion.Ground)
                {
                    var treeProp = new NetLaneProps.Prop()
                    {
                        m_tree = Prefabs.Find<TreeInfo>("Tree2variant"),
                        m_repeatDistance = 30,
                        m_probability = 100,
                    };
                    treeProp.m_position.x = ((i * 2) - 1) * 1.4f;

                    tempProps.Add(treeProp);
                }
                else if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
                {
                    var benchProp1 = new NetLaneProps.Prop()
                    {
                        m_prop = Prefabs.Find<PropInfo>("High Bench"),
                        m_repeatDistance = 80,
                        m_segmentOffset = (i - 1) * -0.5f,
                        m_probability = 100,
                        m_angle = 90 + (i * 180)
                    };
                    benchProp1.m_position.x = ((i * 2) - 1)* 1.8f;

                    tempProps.Add(benchProp1);
                }
                pedLanes[i].m_laneProps.m_props = tempProps.ToArray();
            }

            var laneCollection = new List<NetInfo.Lane>();
            laneCollection.AddRange(bikeLanes);
            laneCollection.AddRange(carLanes);
            laneCollection.AddRange(pedLanes);
            info.m_lanes = laneCollection.ToArray();

            ///////////////////////////
            // AI                    //
            ///////////////////////////
            var pedestrianVanilla = Prefabs.Find<NetInfo>(NetInfos.Vanilla.PED_PAVEMENT);
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var vanillaplayerNetAI = pedestrianVanilla.GetComponent<PlayerNetAI>();
                        var playerNetAI = info.GetComponent<PlayerNetAI>();

                        if (playerNetAI != null)
                        {
                            playerNetAI.m_constructionCost = vanillaplayerNetAI.m_constructionCost * 2;
                            playerNetAI.m_maintenanceCost = vanillaplayerNetAI.m_maintenanceCost * 2;
                        }
                    }
                    break;
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();
            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = false;
                roadBaseAI.m_noiseAccumulation = 3;
                roadBaseAI.m_noiseRadius = 30;
            }

            var roadAI = info.GetComponent<RoadAI>();
            if (roadAI != null)
            {
                roadAI.m_enableZoning = true;
            }
        }

        public void LateBuildUp(NetInfo info, NetInfoVersion version)
        {
            var stoneBollard = PrefabCollection<PropInfo>.FindLoaded($"{Tools.PackageName("BridgePillar")}.CableStay32m_Data");

            var pedLanes = info.m_lanes.Where(pl => pl.m_laneType == NetInfo.LaneType.Pedestrian).ToArray();
            for (var i = 0; i < pedLanes.Length; i++)
            {
                var bollardProp = new NetLaneProps.Prop()
                {
                    m_prop = stoneBollard,
                    m_finalProp = stoneBollard,
                    m_probability = 100,
                    m_segmentOffset = 1,
                    m_minLength = 10,
                    m_endFlagsRequired = NetNode.Flags.Transition
                };
                bollardProp.m_position.x = ((i * 2) - 1) * -3f;
                bollardProp.m_position.y = -0.3f;

                var bollardProp2 = bollardProp.ShallowClone();
                bollardProp2.m_segmentOffset = -1;
                bollardProp2.m_endFlagsRequired = NetNode.Flags.None;
                bollardProp2.m_startFlagsRequired = NetNode.Flags.Transition;

                var bollardProp3 = bollardProp.ShallowClone();
                bollardProp3.m_position.x = ((i * 2) - 1);

                var bollardProp4 = bollardProp2.ShallowClone();
                bollardProp4.m_position.x = ((i * 2) - 1);
                bollardProp2.m_endFlagsRequired = NetNode.Flags.None;
                bollardProp2.m_startFlagsRequired = NetNode.Flags.Transition;

                var bollardProps = new List<NetLaneProps.Prop> { bollardProp, bollardProp2, bollardProp3, bollardProp4 };
                var tempProps = pedLanes[i].m_laneProps.m_props.ToList();
                tempProps.AddRange(bollardProps);
                pedLanes[i].m_laneProps.m_props = tempProps.ToArray();
            }
        }
    }
}
