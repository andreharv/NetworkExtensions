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
    public partial class Avenue6LBusCenterBikeBuilder : Activable, IMultiNetInfoBuilderPart
    {
        public int Order { get { return 25; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_4L; } }
        public string Name { get { return "WideAvenue6LBusCenterBike"; } }
        public string DisplayName { get { return "Six-Lane Road with Bus Line Parking And Bike Lanes"; } }
        public string Description { get { return "A six-lane road with bus and bike lanes. Supports heavy urban traffic."; } }
        public string ShortDescription { get { return "Parking, zoneable, heavy urban traffic"; } }
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
                    Name = "WideAvenue6LBusCenterBike",
                    DisplayName = "Six-Lane Road with Bus Line Parking And Bike Lanes",
                    Description = "A six-lane road with bus and bike lanes. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\WideAvenues\Avenue6LBusCenterBike\thumbnails.png",
                    InfoTooltipPath = @"Roads\WideAvenues\Avenue6LBusCenterBike\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = RExExtendedMenus.ROADS_WIDE,
                    UIOrder = 21,
                    Name = "WideAvenue6LBusCenterBike Decoration Grass",
                    DisplayName = "Six-Lane Road with Bus Line Parking And Bike Lanes",
                    Description = "A six-lane road with bus and bike lanes. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\WideAvenues\Avenue6LBusCenterBike\thumbnails_grass.png",
                    InfoTooltipPath = @"Roads\WideAvenues\Avenue6LBusCenterBike\infotooltip_grass.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = RExExtendedMenus.ROADS_WIDE,
                    UIOrder = 22,
                    Name = "WideAvenue6LBusCenterBike Decoration Trees",
                    DisplayName = "Six-Lane Road with Bus Line Parking And Bike Lanes",
                    Description = "A six-lane road with bus and bike lanes. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\WideAvenues\Avenue6LBusCenterBike\thumbnails_trees.png",
                    InfoTooltipPath = @"Roads\WideAvenues\Avenue6LBusCenterBike\infotooltip_trees.png"
                };
            }

        }
        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var roadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_4L_TUNNEL);
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup40m3mSW1mB2x4mMdnMesh(version);

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

            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition | CitizenInstance.Flags.Underground;
                info.m_class = roadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_XLARGE_ROAD_TUNNEL);
            }
            else
            {
                info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_XLARGE_ROAD);
            }

            // Setting up lanes
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                LanesToAdd = 4,
                LaneWidth = version == NetInfoVersion.Slope ? 2.75f : 3,
                
                CenterLane = CenterLaneType.Median,
                CenterLaneWidth = 2,
                BusStopOffset = 0,
                HasBusStop = false
            });


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

            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

            var leftPed = info.GetLeftRoadShoulder().CloneWithoutStops();
            leftPed.m_width = 2f;
            leftPed.m_position = 4f;


            var rightPed = info.GetRightRoadShoulder().CloneWithoutStops();
            rightPed.m_position = -4f;
            rightPed.m_width = 2f;


            /*
            for (var i = 0; i < leftPedLane.m_laneProps.m_props.Count(); i++)
            {
                Debug.Log(leftPedLane.m_laneProps.m_props[i].m_prop.name.ToLower());

            }*/


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


            leftPedLane.m_width = 2f;
            leftPedLane.m_position = -18.3f;
            rightPedLane.m_width = 2f;
            rightPedLane.m_position = 18.3f;

            rightPed.m_stopType = VehicleInfo.VehicleType.Car;
            leftPed.m_stopType = VehicleInfo.VehicleType.Car;
            leftPedLane.m_stopType = VehicleInfo.VehicleType.None;
            rightPedLane.m_stopType = VehicleInfo.VehicleType.None;




            var centerLane1 = info.GetMedianLane().CloneWithoutStops();
            var centerLane2 = info.GetMedianLane().CloneWithoutStops();
            centerLane1.m_width = 2f;
            centerLane2.m_width = 2f;
            centerLane1.m_position = -4.5f;
            centerLane2.m_position = 4.5f;

       

            var leftPedLaneProps = leftPedLane.m_laneProps.m_props.ToList();
            var rightPedLaneProps = rightPedLane.m_laneProps.m_props.ToList();


            var centerLane1PedLaneProps = centerLane1.m_laneProps.m_props.ToList();
            var centerLane2PedLaneProps = centerLane2.m_laneProps.m_props.ToList();

            //     var trafficLight = Prefabs.Find<PropInfo>("Traffic Light 01");
            //    Debug.Log("trafficLight");
            //     Debug.Log(trafficLight);

            var centerLane1StreetLight = centerLane1PedLaneProps?.FirstOrDefault(p => {
                if (p == null || p.m_prop == null)
                {
                    return false;
                }
                return p.m_prop.name.ToLower().Contains("avenue light");
            });
            if (centerLane1StreetLight != null)
            {
                centerLane1StreetLight.m_finalProp =
                 centerLane1StreetLight.m_prop = Prefabs.Find<PropInfo>("New Street Light");
                centerLane1StreetLight.m_angle = 180;
                var lefttLigth = centerLane1StreetLight.ShallowClone();
                lefttLigth.m_position = new Vector3(-1.25f, 0, 0);
               // lefttLigth.m_repeatDistance = lefttLigth.m_repeatDistance / 2;
                leftPedLaneProps.AddProp(lefttLigth);
            }

            var centerLane2StreetLight = centerLane2PedLaneProps?.FirstOrDefault(p =>
            {
                if (p == null || p.m_prop == null)
                {
                    return false;
                }
                return p.m_prop.name.ToLower().Contains("avenue light");
            });
            if (centerLane2StreetLight != null)
            {
                centerLane2StreetLight.m_finalProp =
                 centerLane2StreetLight.m_prop = Prefabs.Find<PropInfo>("New Street Light");
                centerLane2StreetLight.m_angle = 0;
                var rightLigth = centerLane2StreetLight.ShallowClone();
                rightLigth.m_position = new Vector3(1.25f, 0, 0);
             //   rightLigth.m_repeatDistance = rightLigth.m_repeatDistance / 2;
                rightPedLaneProps.AddProp(rightLigth);
            }

            var ind = 0;
            var indped = 0;
            centerLane1PedLaneProps?.ForEach(p => {
                if (p == null || p.m_prop == null)
                {
                    return;
                }

                if (p.m_prop.name.ToLower().Contains("pedestrian"))
                {
                    indped++;
                        p.m_position = new Vector3(-0.9f, 0, 0);
                        p.m_angle = 270;
                    return;
                }

                if (p.m_prop.name.ToLower().Contains("mirror"))
                {
                    ind++;

                    if (ind == 1)
                    {
                        p.m_finalProp =
                        p.m_prop = Prefabs.Find<PropInfo>("Traffic Light Pedestrian");
                    }
                    else
                    {
                        p.m_finalProp =
                        p.m_prop = Prefabs.Find<PropInfo>("Traffic Light 02");
                        p.m_position = new Vector3(.9f, 0, 0);
                    }
                }
            });

            ind = 0;
            indped = 0;
            centerLane2PedLaneProps?.ForEach(p => {
                if (p == null || p.m_prop == null)
                {
                    return;
                }

                if (p.m_prop.name.ToLower().Contains("pedestrian"))
                {
                    indped++;
                        p.m_position = new Vector3(0.9f, 0, 0);
                        p.m_angle = 90;
                 return;
                }

                if (p.m_prop.name.ToLower().Contains("mirror"))
                {
                    ind++;

                    if (ind == 2)
                    {
                        p.m_finalProp =
                        p.m_prop = Prefabs.Find<PropInfo>("Traffic Light Pedestrian");
                    }
                    else
                    {
                        p.m_finalProp =
                        p.m_prop = Prefabs.Find<PropInfo>("Traffic Light 02");
                        p.m_position = new Vector3(-.9f, 0, 0);
                    }
                }
            });

            if (centerLane1PedLaneProps != null)
            {
                centerLane1PedLaneProps.RemoveProps("avenue light");
                centerLane1PedLaneProps.RemoveProps("sign");
                centerLane1PedLaneProps.RemoveProps("50 Speed Limit");
            }
            if (centerLane2PedLaneProps != null)
            {
                centerLane2PedLaneProps.RemoveProps("avenue light");
        
                centerLane2PedLaneProps.RemoveProps("sign");
                centerLane2PedLaneProps.RemoveProps("50 Speed Limit");
            }
          /*  
            if (centerLane1?.m_laneProps != null && centerLane1PedLaneProps != null)
            {
                centerLane1.m_laneProps.m_props = centerLane1PedLaneProps.ToArray();
            }

            if (centerLane2?.m_laneProps != null && centerLane2PedLaneProps != null)
            {
                centerLane2.m_laneProps.m_props = centerLane2PedLaneProps.ToArray();
            }
       */

            //var centerLaneProps = new List<NetLaneProps.Prop>();
            /*if (version == NetInfoVersion.GroundTrees)
            {
                var treeProp = new NetLaneProps.Prop()
                {
                    m_tree = Prefabs.Find<TreeInfo>("Tree2variant"),
                    m_repeatDistance = 50,
                    m_probability = 100,
                };
                treeProp.m_position.x = 0;
                centerLane1PedLaneProps.Add(treeProp.ShallowClone());
                centerLane2PedLaneProps.Add(treeProp.ShallowClone());
            }
            */
            centerLane1.m_laneProps.m_props = centerLane1PedLaneProps.ToArray();
            centerLane2.m_laneProps.m_props = centerLane2PedLaneProps.ToArray();

            leftPedLaneProps.ForEach(prop =>
            {
                prop.m_position = new Vector3(0.8f, 0, 0);
            });


            rightPedLaneProps.ForEach(prop =>
            {
                prop.m_position = new Vector3(-0.8f, 0, 0);
            });
            leftPedLane.m_laneProps.m_props = leftPedLaneProps.ToArray();
            rightPedLane.m_laneProps.m_props = rightPedLaneProps.ToArray();


            var pedLanes = new List<NetInfo.Lane>();
            if (version == NetInfoVersion.Ground)
            {
                pedLanes.Add(rightPed);
                pedLanes.Add(leftPed);
            }
            pedLanes.Add(leftPedLane);
            pedLanes.Add(rightPedLane);

            var tempLanes = new List<NetInfo.Lane>();
            tempLanes.Add(centerLane1);
            tempLanes.Add(centerLane2);
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

