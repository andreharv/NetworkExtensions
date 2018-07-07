using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Props;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using UnityEngine;
#if DEBUG
using Debug = Transit.Framework.Debug;
#endif
namespace Transit.Addon.RoadExtensions.Roads.WideAvenues.Avenue6LBusCenterBike
{
    public partial class Avenue6LBusCenterBikeBuilderEndNode : Activable, IMultiNetInfoBuilderPart
    {
        public int Order { get { return 25; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public string Name { get { return "WideAvenue6LBusCenterBikeEndNode"; } }
        public string DisplayName { get { return "Eight-Lane Road Special"; } }
        public string Description { get { return "An eight-lane road with paved median. Supports heavy urban traffic."; } }
        public string ShortDescription { get { return "No parking, zoneable, heavy urban traffic"; } }
        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground | NetInfoVersion.GroundGrass | NetInfoVersion.GroundTrees; }
        }

        public IEnumerable<IMenuItemBuilder> MenuItemBuilders
        {
            get
            {
                yield return new MenuItemBuilder
                {
                    UICategory = RExExtendedMenus.ROADS_WIDE,
                    UIOrder = 20,
                    Name = "WideAvenue6LBusCenterBikeEndNode",
                    DisplayName = "Four-Devided-Lane Avenue With 4 Parking",
                    Description = "A basic two lane road with a median and no parkings spaces. Supports local traffic.",
                    ThumbnailsPath = @"Roads\WideAvenues\Avenue6LBusCenterBike\Special\endnode.png",
                    InfoTooltipPath = @"Roads\WideAvenues\Avenue6LBusCenterBike\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = RExExtendedMenus.ROADS_WIDE,
                    UIOrder = 21,
                    Name = "WideAvenue6LBusCenterBikeEndNode Decoration Grass",
                    DisplayName = "Four-Devided-Lane Avenue With 4 Parking and Grass",
                    Description = "A basic two lane road with a grass median and with parkings spaces. Supports local traffic.",
                    ThumbnailsPath = @"Roads\WideAvenues\Avenue6LBusCenterBike\Special\endnode_grass.png",
                    InfoTooltipPath = @"Roads\WideAvenues\Avenue6LBusCenterBike\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = RExExtendedMenus.ROADS_WIDE,
                    UIOrder = 22,
                    Name = "WideAvenue6LBusCenterBikeEndNode Decoration Trees",
                    DisplayName = "Four-Devided-Lane Avenue With 4 Parking",
                    Description = "A basic two lane road with a grass median, trees and with parkings spaces. Supports local traffic.",
                    ThumbnailsPath = @"Roads\WideAvenues\Avenue6LBusCenterBike\Special\endnode_trees.png",
                    InfoTooltipPath = @"Roads\WideAvenues\Avenue6LBusCenterBike\infotooltip.png"
                };
            }

        }
        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup40m3mSW1mB2x4mMdnSpecialMesh(version, SpecailSegments.EndNode);
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = true;
            info.m_pavementWidth = (version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel ? 4 : 3);
            info.m_halfWidth = (version == NetInfoVersion.Tunnel ? 17 : 20);


            info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_XLARGE_ROAD);


            var config = new LanesConfiguration
            {
                IsTwoWay = false,
                LanesToAdd = 7,
                LaneWidth = version == NetInfoVersion.Slope ? 2.75f : 3,

                CenterLane = CenterLaneType.Median,
                CenterLaneWidth = 2,
                BusStopOffset = 0,
                HasBusStop = false
            };


            // Setting up lanes
            info.SetRoadLanes(version, config);

            var medianLane = new NetInfo.Lane();
            RoadHelper.SetupMedianLane(medianLane, config, version);

            var carLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).ToList();
            var pedkLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();
            var parking = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Parking).ToList();
            for (var i = 0; i < parking.Count; i++)
            {
                parking[i].m_position = i == 0 ? -13f : 13f;
            }


            carLanes[0].m_direction = NetInfo.Direction.Backward;
            carLanes[0].m_finalDirection = NetInfo.Direction.Backward;
            carLanes[0].m_position = -16f;
            carLanes[0].m_width = 2f;
            carLanes[0].m_vehicleType = VehicleInfo.VehicleType.Bicycle;


            carLanes[1].m_direction = NetInfo.Direction.Backward;
            carLanes[1].m_finalDirection = NetInfo.Direction.Backward;
            carLanes[1].m_stopType = VehicleInfo.VehicleType.None;
            carLanes[1].m_position = -10.5f;

            carLanes[2].m_position = -7.5f;
            carLanes[2].m_direction = NetInfo.Direction.Backward;
            carLanes[2].m_finalDirection = NetInfo.Direction.Backward;
            carLanes[2].m_stopType = VehicleInfo.VehicleType.None;


            carLanes[3].m_position = -1.5f;
            carLanes[3].m_direction = NetInfo.Direction.Backward;
            carLanes[3].m_finalDirection = NetInfo.Direction.Backward;
            carLanes[3].m_laneType = NetInfo.LaneType.TransportVehicle;
            carLanes[3].m_stopType = VehicleInfo.VehicleType.Car;
            var tempProps = carLanes[3].m_laneProps.m_props.ToList();
            tempProps.RemoveProps("arrow");
            carLanes[3].m_laneProps.m_props = tempProps.ToArray();
            carLanes[3].m_stopType = VehicleInfo.VehicleType.None;


            carLanes[4].m_position = 1.5f;
            carLanes[4].m_direction = NetInfo.Direction.Forward;
            carLanes[4].m_finalDirection = NetInfo.Direction.Forward;
            carLanes[4].m_laneType = NetInfo.LaneType.TransportVehicle;
            carLanes[4].m_stopType = VehicleInfo.VehicleType.Car;
            tempProps = carLanes[4].m_laneProps.m_props.ToList();
            tempProps.RemoveProps("arrow");
            carLanes[4].m_laneProps.m_props = tempProps.ToArray();

            BusRoads.BusRoadsHelper.SetBusLaneProps(carLanes[3]);
            BusRoads.BusRoadsHelper.SetBusLaneProps(carLanes[4]);


            carLanes[5].m_position = 7.5f;
            carLanes[5].m_direction = NetInfo.Direction.Forward;
            carLanes[5].m_finalDirection = NetInfo.Direction.Forward;
            carLanes[5].m_stopType = VehicleInfo.VehicleType.None;

            carLanes[6].m_position = 10.5f;
            carLanes[6].m_direction = NetInfo.Direction.Forward;
            carLanes[6].m_finalDirection = NetInfo.Direction.Forward;
            carLanes[6].m_stopType = VehicleInfo.VehicleType.None;


            carLanes[7].m_direction = NetInfo.Direction.Forward;
            carLanes[7].m_finalDirection = NetInfo.Direction.Forward;
            carLanes[7].m_stopType = VehicleInfo.VehicleType.None;
            carLanes[7].m_position = 16f;
            carLanes[7].m_width = 2f;
            carLanes[7].m_vehicleType = VehicleInfo.VehicleType.Bicycle;

            carLanes[7].SetBikeLaneProps();
            carLanes[0].SetBikeLaneProps();


            carLanes[8].m_position = 4.5f;
            carLanes[8].m_direction = NetInfo.Direction.Forward;
            carLanes[8].m_finalDirection = NetInfo.Direction.Forward;
            carLanes[8].m_stopType = VehicleInfo.VehicleType.None;


            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

            var leftPed = info.GetLeftRoadShoulder().CloneWithoutStops();
            leftPed.m_width = 2f;
            leftPed.m_position = 4f;

            var rightPed = info.GetRightRoadShoulder().CloneWithoutStops();
            rightPed.m_position = -4f;
            rightPed.m_width = 2f;


            tempProps = leftPed.m_laneProps.m_props.ToList();
            tempProps.RemoveProps("light");
            tempProps.RemoveProps("limit");
            tempProps.RemoveProps("bus");
            tempProps.RemoveProps("random");
            tempProps.RemoveTrees("tree");
            leftPed.m_laneProps.m_props = tempProps.ToArray();


            tempProps = leftPedLane.m_laneProps.m_props.ToList();
            tempProps.RemoveProps("avenue");
            tempProps.RemoveTrees("tree");
            leftPedLane.m_laneProps.m_props = tempProps.ToArray();
            leftPedLane.m_width = 2f;
            leftPedLane.m_position = -18.3f;
            rightPedLane.m_width = 2f;
            rightPedLane.m_position = 17.5f;



            tempProps = rightPed.m_laneProps.m_props.ToList();
            tempProps.RemoveProps("light");
            tempProps.RemoveProps("bus");
            tempProps.RemoveProps("limit");
            tempProps.RemoveProps("random");
            tempProps.RemoveTrees("tree");
            rightPed.m_laneProps.m_props = tempProps.ToArray();


            tempProps = rightPedLane.m_laneProps.m_props.ToList();
            tempProps.RemoveProps("avenue");
            tempProps.RemoveTrees("tree");
            rightPedLane.m_laneProps.m_props = tempProps.ToArray();
            rightPedLane.m_position = rightPedLane.m_position + .75f;

            rightPed.m_stopType = VehicleInfo.VehicleType.Car;
            leftPed.m_stopType = VehicleInfo.VehicleType.Car;
            leftPedLane.m_stopType = VehicleInfo.VehicleType.None;
            rightPedLane.m_stopType = VehicleInfo.VehicleType.None;



            var centerLane1 = medianLane.CloneWithoutStops();
            var centerLane2 = medianLane.CloneWithoutStops();
            centerLane1.m_width = 2f;
            centerLane2.m_width = 2f;
            centerLane1.m_position = -4.5f;
            centerLane2.m_position = 4.5f;

            var centerLane1PedLaneProps = centerLane1.m_laneProps.m_props.ToList();
            var centerLane2PedLaneProps = centerLane2.m_laneProps.m_props.ToList();

            var leftPedLaneProps = leftPedLane.m_laneProps.m_props.ToList();
            var rightPedLaneProps = rightPedLane.m_laneProps.m_props.ToList();


            var leftPedLanePedLaneProps = leftPedLane.m_laneProps.m_props.ToList();

            //     var trafficLight = Prefabs.Find<PropInfo>("Traffic Light 01");
            //    Debug.Log("trafficLight");
            //     Debug.Log(trafficLight);

            var streetLight = leftPedLanePedLaneProps?.FirstOrDefault(p => {
                if (p == null || p.m_prop == null)
                {
                    return false;
                }
                return p.m_prop.name.ToLower().Contains("street light");
            });
            if (streetLight != null)
            {
                streetLight.m_finalProp =
                streetLight.m_prop = Prefabs.Find<PropInfo>("New Street Light");
                streetLight.m_angle = 180;
                var lefttLigth = streetLight.ShallowClone();
                var rightLigth = streetLight.ShallowClone();
                leftPedLaneProps.RemoveProps("street light");
                lefttLigth.m_position = new Vector3(2.25f, 0, 0);
                //lefttLigth.m_repeatDistance = lefttLigth.m_repeatDistance / 2;
                leftPedLaneProps.AddProp(lefttLigth);
                //rightLigth.m_repeatDistance = rightLigth.m_repeatDistance / 2;
                rightLigth.m_angle = 0;
                rightLigth.m_position = new Vector3(-2.25f, 0, 0);
                rightPedLaneProps.AddProp(rightLigth);
            }

            //var centerLaneProps = new List<NetLaneProps.Prop>();
            if (version == NetInfoVersion.GroundTrees)
            {
                var treeProp = new NetLaneProps.Prop()
                {
                    m_tree = Prefabs.Find<TreeInfo>("Tree2variant"),
                    m_repeatDistance = 10,
                    m_probability = 100,
                };
                treeProp.m_position.x = 0;
                centerLane1PedLaneProps.Add(treeProp.ShallowClone());
            
                //    centerLane2PedLaneProps.Add(treeProp.ShallowClone());
            }
          
           
            leftPedLaneProps.ForEach(prop =>
            {
                if (prop.m_prop.name.ToLower().Contains("traffic"))
               {
                    prop.m_finalProp =
                    prop.m_prop = Prefabs.Find<PropInfo>("Traffic Light Pedestrian");
                    var traffic = prop.ShallowClone();
                    traffic.m_position = new Vector3(0, 0, 0);
                    traffic.m_finalProp =
                    traffic.m_prop = Prefabs.Find<PropInfo>("Traffic Light 02 Mirror");
                    centerLane1PedLaneProps.AddProp(traffic);
                }
                prop.m_position = new Vector3(0.8f, 0, 0);
            });

            rightPedLaneProps.ForEach(prop =>
            {
                if (prop.m_prop.name.ToLower().Contains("traffic"))
                {
                    prop.m_finalProp =
                    prop.m_prop = Prefabs.Find<PropInfo>("Traffic Light 02");
                }
                prop.m_position = new Vector3(-0.8f, 0, 0);

            });
            leftPedLane.m_laneProps.m_props = leftPedLaneProps.ToArray();
            rightPedLane.m_laneProps.m_props = rightPedLaneProps.ToArray();

            centerLane1.m_laneProps.m_props = centerLane1PedLaneProps.ToArray();
            //centerLane2.m_laneProps.m_props = centerLane2PedLaneProps.ToArray();

            var pedLanes = new List<NetInfo.Lane>();
            if (version == NetInfoVersion.Ground)
            {
                pedLanes.Add(rightPed);
            }
        //    pedLanes.Add(leftPed);
            pedLanes.Add(leftPedLane);
            pedLanes.Add(rightPedLane);

            var tempLanes = new List<NetInfo.Lane>();
            tempLanes.Add(centerLane1);
          //  tempLanes.Add(centerLane2);
            tempLanes.AddRange(pedLanes);
            tempLanes.AddRange(carLanes);
            tempLanes.AddRange(parking);
            info.m_lanes = tempLanes.ToArray();

            // AI
            var owPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 4; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 4; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }

        public void LateBuildUp(NetInfo info, NetInfoVersion version)
        {
            if (version == NetInfoVersion.Bridge)
            {
                var bridgePillar = PrefabCollection<BuildingInfo>.FindLoaded($"{Tools.PackageName("BridgePillar")}.CableStay32m_Data");
                if (bridgePillar == null)
                {
                    Debug.Log($"{info.name}: CableStay32m Pillar not found!");
                }
                else
                { 
                    var bridgeAI = info.GetComponent<RoadBridgeAI>();
                    if (bridgeAI != null)
                    {
                        bridgeAI.m_doubleLength = true;
                        bridgeAI.m_bridgePillarInfo = null;
                        bridgeAI.m_middlePillarInfo = bridgePillar;
                        bridgeAI.m_middlePillarOffset = 58;
                    }
                }
            }
        }
    }
}

