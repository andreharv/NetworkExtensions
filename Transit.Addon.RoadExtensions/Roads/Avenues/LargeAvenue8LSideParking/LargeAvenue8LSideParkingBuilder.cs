using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Addon.RoadExtensions.Props;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using TransitPlus.Addon.RoadExtensions.Roads.Common;
using UnityEngine;
#if DEBUG
using Debug = Transit.Framework.Debug;
#endif
namespace TransitPlus.Addon.RoadExtensions.Roads.Avenues.LargeAvenue8LSideParking
{
    public partial class LargeAvenue8LSideParkingBuilder : Activable, IMultiNetInfoBuilderPart
    {
        public int Order { get { return 25; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_4L; } }
        public string Name { get { return "FourDevidedLaneAvenue4Parking"; } }
        public string DisplayName { get { return "Four-Devided-Lane Avenue With Parking"; } }
        public string Description { get { return "A four-lane road with paved median. Supports heavy urban traffic."; } }
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
                    UIOrder = 25,
                    Name = "FourDevidedLaneAvenue4Parking",
                    DisplayName = "Four-Devided-Lane Avenue With Parking",
                    Description = "A four-lane road with paved median. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue8LSideParking\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue8LSideParking\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 26,
                    Name = "FourDevidedLaneAvenue4Parking Decoration Grass",
                    DisplayName = "Four-Devided-Lane Avenue With Parking and Grass",
                    Description = "A four-lane road with paved median. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue8LSideParking\thumbnails_grass.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue8LSideParking\infotooltip_grass.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 27,
                    Name = "FourDevidedLaneAvenue4Parking Decoration Trees",
                    DisplayName = "Four-Devided-Lane Avenue With 4 Parking",
                    Description = "A four-lane road with paved median. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue8LSideParking\thumbnails_trees.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue8LSideParking\infotooltip_trees.png"
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
            info.Setup32m3mSW2x3mMdnMesh(version);

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


            info.m_class = roadInfo.m_class.Clone("FourDevidedLaneAvenue4Parking");
            info.m_canCrossLanes = false;
            // Setting up lanes
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
              //  LanesToAdd = 2,
                LaneWidth = version == NetInfoVersion.Slope ? 2.75f : 3,
                PedPropOffsetX = version == NetInfoVersion.Slope ? 1.5f : 1f,
                CenterLane = CenterLaneType.Median,
                CenterLaneWidth = 2,
                BusStopOffset = 0f
            });
        

            var carLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).ToList();
            var pedkLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();
            var parking = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Parking).ToList();

            var parkLane1 = parking[0].ShallowClone();
            parkLane1.m_width = 2;
            parkLane1.m_position = -4f;
            parking.Add(parkLane1);


            var parkLane2 = parking[1].ShallowClone();
            parkLane2.m_width = 2;
            parkLane2.m_position = 4f;
            parking.Add(parkLane2);



     
            carLanes[0].m_direction = NetInfo.Direction.Backward;
            carLanes[0].m_finalDirection = NetInfo.Direction.Backward;
            carLanes[0].m_position = -9.5f;
            carLanes[0].m_speedLimit = .2f;
            // RoadHelper.SetupAnyDirectionProps(carLanes[0], carLanes[2]);


        ///    RoadHelper.SetupAnyDirectionProps(carLanes[1], carLanes[0]);

            carLanes[1].m_position =  -1.5f;
            carLanes[1].m_direction = NetInfo.Direction.Backward;
            carLanes[1].m_finalDirection = NetInfo.Direction.Backward;


            carLanes[2].m_position = 1.5f;
            carLanes[2].m_direction = NetInfo.Direction.Forward;
            carLanes[2].m_finalDirection = NetInfo.Direction.Forward;


            carLanes[3].m_position = 9.5f;
            carLanes[3].m_speedLimit = .2f;
            carLanes[3].m_direction = NetInfo.Direction.Forward;
            carLanes[3].m_finalDirection = NetInfo.Direction.Forward;

            var tempProps = carLanes[0].m_laneProps.m_props.ToList();
            tempProps.RemoveProps("arrow");
            carLanes[0].m_laneProps.m_props = tempProps.ToArray();

            tempProps = carLanes[3].m_laneProps.m_props.ToList();
            tempProps.RemoveProps("arrow");
            carLanes[3].m_laneProps.m_props = tempProps.ToArray();

      

            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();
       

            var leftPed = info.GetLeftRoadShoulder().FullClone();
            leftPed.m_width = 1f;
            leftPed.m_position = -5.5f;
            leftPed.m_stopType = VehicleInfo.VehicleType.Car;

            var rightPed = info.GetRightRoadShoulder().FullClone();      
            rightPed.m_position = 5.5f;
            rightPed.m_width = 1f;
            rightPed.m_stopType = VehicleInfo.VehicleType.Car;

              var props = rightPed.m_laneProps.m_props.ToList();
            props.RemoveProps("light");
            props.RemoveProps("limit");
            props.RemoveProps("random");
            rightPed.m_laneProps.m_props = props.ToArray();

            props = leftPed.m_laneProps.m_props.ToList();
            props.RemoveProps("light");
            props.RemoveProps("limit");
            props.RemoveProps("random");
            leftPed.m_laneProps.m_props = props.ToArray();

        


            var centerLane1 = info.GetMedianLane().CloneWithoutStops();
            var centerLane2 = info.GetMedianLane().CloneWithoutStops();
            centerLane1.m_width = 2f;
            centerLane2.m_width = 2f;
            centerLane1.m_position = -6.5f;
            centerLane2.m_position = 6.5f;



            var leftPedLaneProps = leftPedLane.m_laneProps.m_props.ToList();
            var rightPedLaneProps = rightPedLane.m_laneProps.m_props.ToList();

            var centerLane1PedLaneProps = centerLane1.m_laneProps.m_props.ToList();
            var centerLane2PedLaneProps = centerLane2.m_laneProps.m_props.ToList();

            var centerLane1StreetLight = centerLane1PedLaneProps?.FirstOrDefault(p => {
                if (p == null || p.m_prop == null)
                {
                    return false;
                }
                return p.m_prop.name.ToLower().Contains("avenue light");
            });

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
                    if (indped == 1)
                    {
                        p.m_position = new Vector3(-0.6f, 0, 0);
                        p.m_angle = 270;
                    }
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


            // var centerLaneProps = new List<NetLaneProps.Prop>();
            /*  if (version == NetInfoVersion.GroundTrees)
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
            //      centerLane2PedLaneProps.RemoveProps("mirror");
            //       centerLane1PedLaneProps.RemoveProps("mirror");
            centerLane1.m_laneProps.m_props = centerLane1PedLaneProps.ToArray();
            centerLane2.m_laneProps.m_props = centerLane2PedLaneProps.ToArray();

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
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 2; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 2; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }

    }
}

