using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Addon.RoadExtensions.Props;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using UnityEngine;
using ColossalFramework.Plugins;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif
namespace Transit.Addon.RoadExtensions.Roads.Avenues.LargeAvenue8L2BusLanes
{
    public partial class LargeAvenue8L2BusLanesBuilder : Activable, IMultiNetInfoBuilderPart
    {
        public int Order { get { return 28; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_4L; } }
        public string Name { get { return "FourDevidedLaneAvenue2Bus"; } }
        public string DisplayName { get { return "Six-Lane Road with Bus Line And Parking"; } }
        public string Description { get { return "A six-lane road with bus lane. Supports heavy urban traffic."; } }
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
                    UICategory = "RoadsMedium",
                    UIOrder = 28,
                    Name = "FourDevidedLaneAvenue2Bus",
                    DisplayName = "Six-Lane Road with Bus Line And Parking",
                    Description = "A six-lane road with bus lane. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue8L2BusLanes\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue8L2BusLanes\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 30,
                    Name = "FourDevidedLaneAvenue2Bus Decoration Trees",
                    DisplayName = "Six-Lane Road with Bus Line And Parking ",
                    Description = "A six-lane road with bus lane. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue8L2BusLanes\thumbnails_trees.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue8L2BusLanes\infotooltip_trees.png"
                };

                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 29,
                    Name = "FourDevidedLaneAvenue2Bus Decoration Grass",
                    DisplayName = "Six-Lane Road with Bus Line And Parking",
                    Description = "A six-lane road with bus lane. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue8L2BusLanes\thumbnails_grass.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue8L2BusLanes\infotooltip_grass.png"
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
            info.Setup32m3mSW2x3mMdnBusMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = true;
            info.m_pavementWidth = (version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel ? 4 : 3);
            info.m_halfWidth = (version == NetInfoVersion.Tunnel ? 17 : 16);

            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition | CitizenInstance.Flags.Underground;
                info.m_class = roadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_XLARGE_ROAD_TUNNEL);
            }
            else
            {
                info.m_class = info.m_class.Clone("NEXTFourDevidedLaneBusAvenue4Parking" + version.ToString());
            }
            info.m_canCrossLanes = false;
            // Setting up lanes
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                LanesToAdd = 2,
                LaneWidth = version == NetInfoVersion.Slope ? 2.75f : 3,
                PedPropOffsetX = version == NetInfoVersion.Slope ? 1.5f : 1f,
                CenterLane = CenterLaneType.Median,
                CenterLaneWidth = 2,
                BusStopOffset = 0,
                HasBusStop = false
            });

            var carLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).ToList();
            var pedkLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();
            var parking = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Parking).ToList();




            carLanes[0].m_direction = NetInfo.Direction.Backward;
            carLanes[0].m_finalDirection = NetInfo.Direction.Backward;
            carLanes[0].m_position = -9.5f;

            carLanes[1].m_position = -6.6f;
            carLanes[1].m_direction = NetInfo.Direction.Backward;
            carLanes[1].m_finalDirection = NetInfo.Direction.Backward;
            carLanes[1].m_stopType = VehicleInfo.VehicleType.None;

            carLanes[2].m_position = -1.5f;
            carLanes[2].m_direction = NetInfo.Direction.Backward;
            carLanes[2].m_finalDirection = NetInfo.Direction.Backward;
            carLanes[2].m_laneType = NetInfo.LaneType.TransportVehicle;
            carLanes[2].m_stopType = VehicleInfo.VehicleType.Car;
            var tempProps = carLanes[2].m_laneProps.m_props.ToList();
            tempProps.RemoveProps("arrow");
            carLanes[2].m_laneProps.m_props = tempProps.ToArray();
            carLanes[2].m_stopType = VehicleInfo.VehicleType.None;


            carLanes[3].m_position = 1.5f;
            carLanes[3].m_direction = NetInfo.Direction.Forward;
            carLanes[3].m_finalDirection = NetInfo.Direction.Forward;
            carLanes[3].m_laneType = NetInfo.LaneType.TransportVehicle;
            carLanes[3].m_stopType = VehicleInfo.VehicleType.Car;
            tempProps = carLanes[3].m_laneProps.m_props.ToList();
            tempProps.RemoveProps("arrow");
            carLanes[3].m_laneProps.m_props = tempProps.ToArray();

            BusRoads.BusRoadsHelper.SetBusLaneProps(carLanes[2]);
            BusRoads.BusRoadsHelper.SetBusLaneProps(carLanes[3]);


            carLanes[4].m_position = 6.6f;
            //  carLanes[4].m_speedLimit = .2f;
            carLanes[4].m_direction = NetInfo.Direction.Forward;
            carLanes[4].m_finalDirection = NetInfo.Direction.Forward;
            carLanes[4].m_stopType = VehicleInfo.VehicleType.None;


            carLanes[5].m_position = 9.5f;
            carLanes[5].m_direction = NetInfo.Direction.Forward;
            carLanes[5].m_finalDirection = NetInfo.Direction.Forward;
            carLanes[5].m_stopType = VehicleInfo.VehicleType.None;

            


            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

            var leftPed = info.GetLeftRoadShoulder().CloneWithoutStops();
            leftPed.m_width = 1f;
            leftPed.m_position = -3.8f;
            


            tempProps = leftPed.m_laneProps.m_props.ToList();
            tempProps.RemoveProps("light");
            tempProps.RemoveProps("limit");
            tempProps.RemoveProps("random");
            leftPed.m_laneProps.m_props = tempProps.ToArray();
            
            
        
            var rightPed = info.GetRightRoadShoulder().CloneWithoutStops();
            rightPed.m_position = 3.8f;
            rightPed.m_width = 1f;



            tempProps = rightPed.m_laneProps.m_props.ToList();
            tempProps.RemoveProps("light");
            tempProps.RemoveProps("limit");
            tempProps.RemoveProps("random");
            rightPed.m_laneProps.m_props = tempProps.ToArray();

           

            rightPed.m_stopType = VehicleInfo.VehicleType.Car;
            leftPed.m_stopType = VehicleInfo.VehicleType.Car;
            leftPedLane.m_stopType = VehicleInfo.VehicleType.None;
            rightPedLane.m_stopType = VehicleInfo.VehicleType.None;



  
            var centerLane1 = info.GetMedianLane().CloneWithoutStops();
            var centerLane2 = info.GetMedianLane().CloneWithoutStops();
            centerLane1.m_width = 1f;
            centerLane2.m_width = 1f;
            centerLane1.m_position = -4.3f;
            centerLane2.m_position = 4.3f;
       
            var leftPedLaneProps = leftPed.m_laneProps.m_props.ToList();
            var rightPedLaneProps = rightPed.m_laneProps.m_props.ToList();


            var centerLane1PedLaneProps = centerLane1.m_laneProps.m_props.ToList();
            var centerLane2PedLaneProps = centerLane2.m_laneProps.m_props.ToList();

            if (version == NetInfoVersion.GroundTrees)
            {
                var treeProp = new NetLaneProps.Prop()
                {
                    m_tree = Prefabs.Find<TreeInfo>("Tree2variant"),
                    m_repeatDistance = 30,
                    m_probability = 100,
                };
                treeProp.m_position.x = 0;
                centerLane1PedLaneProps.Add(treeProp.ShallowClone());
                centerLane2PedLaneProps.Add(treeProp.ShallowClone());
            }


            var centerLane1StreetLight = centerLane1PedLaneProps?.FirstOrDefault(p => {
                 if (p == null || p.m_prop == null)
                 {
                     return false;
                 }
                 return p.m_prop.name.ToLower().Contains("avenue light");
             });


            var centerLane1TrafficLight = centerLane1PedLaneProps?.FirstOrDefault(p => {
                if (p == null || p.m_prop == null)
                {
                    return false;
                }
                return p.m_prop.name.ToLower().Contains("traffic light");
            });

            if (centerLane1StreetLight != null)
             {
                 centerLane1StreetLight.m_finalProp =
                  centerLane1StreetLight.m_prop = Prefabs.Find<PropInfo>(MediumAvenueSideLightBuilder.NAME);
                centerLane1StreetLight.m_angle = 180;
                var lefttLigth = centerLane1StreetLight.ShallowClone();
                lefttLigth.m_position = new Vector3(-9.8f, 0, 0);
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
                  centerLane2StreetLight.m_prop = Prefabs.Find<PropInfo>(MediumAvenueSideLightBuilder.NAME);
                centerLane2StreetLight.m_angle = 0;
                var rightLigth = centerLane2StreetLight.ShallowClone();
                rightLigth.m_position = new Vector3(9.8f,0, 0);
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
                //     centerLane1PedLaneProps.RemoveProps("light");
                centerLane1PedLaneProps.RemoveProps("bus");
                centerLane1PedLaneProps.RemoveProps("avenue side");
                centerLane1PedLaneProps.RemoveProps("50 Speed Limit");
            }
            if (centerLane2PedLaneProps != null)
            {
                // centerLane2PedLaneProps.RemoveProps("light");
                centerLane2PedLaneProps.RemoveProps("bus");
                centerLane2PedLaneProps.RemoveProps("avenue side");
                centerLane2PedLaneProps.RemoveProps("50 Speed Limit");
            }
            if (centerLane1?.m_laneProps != null && centerLane1PedLaneProps != null)
            {
                centerLane1.m_laneProps.m_props = centerLane1PedLaneProps.ToArray();
            }

            if (centerLane2?.m_laneProps != null && centerLane2PedLaneProps != null)
            {
                centerLane2.m_laneProps.m_props = centerLane2PedLaneProps.ToArray();
            }
            //var centerLaneProps = new List<NetLaneProps.Prop>();
           
            centerLane1.m_laneProps.m_props = centerLane1PedLaneProps.ToArray();
            centerLane2.m_laneProps.m_props = centerLane2PedLaneProps.ToArray();

            leftPed.m_laneProps.m_props = leftPedLaneProps.ToArray();
            rightPed.m_laneProps.m_props = rightPedLaneProps.ToArray();


            var pedLanes = new List<NetInfo.Lane>();
            pedLanes.Add(rightPed);
            pedLanes.Add(leftPed);
            pedLanes.Add(leftPedLane);
            pedLanes.Add(rightPedLane);
            //carLanes[4].m_position += 1;
            var tempLanes = new List<NetInfo.Lane>();
            tempLanes.Add(centerLane1);
            tempLanes.Add(centerLane2);
            tempLanes.AddRange(pedLanes);
          //  tempLanes.AddRange(pedkLanes);
            tempLanes.AddRange(carLanes);
            tempLanes.AddRange(parking);
            info.m_lanes = tempLanes.ToArray();

       
           

            // AI
            var owPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 3; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 3; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }

    }
}

