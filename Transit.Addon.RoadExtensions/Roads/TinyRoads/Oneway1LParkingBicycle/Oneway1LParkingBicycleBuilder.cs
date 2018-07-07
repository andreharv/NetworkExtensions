using System;
using System.Linq;
using System.Collections.Generic;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.TinyRoads.Oneway1LParkingBicycle
{
    public class Oneway1LParkingBicycleBuilder : Activable, INetInfoBuilderPart, INetInfoLateBuilder
    {
        public const string NAME = "One-Lane Oneway With Parking and Bicycle lane";

        public int Order { get { return 6; } }
        public int UIOrder { get { return 15; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public string Name { get { return NAME; } }
        public string DisplayName { get { return NAME; } }
        public string CodeName { get { return "Oneway1LParkingBicycle"; } }
        public string Description { get { return "A one-lane, oneway road suitable for neighborhood traffic. Has parking spaces and bicycle lane"; } }
        public string ShortDescription { get { return "Zoneable, parking, neighborhood traffic"; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_TINY; } }

        public string ThumbnailsPath { get { return @"Roads\TinyRoads\Oneway1LParkingBicycle\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\TinyRoads\Oneway1LParkingBicycle\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L);

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup8m1mSWMesh(version, LanesLayoutStyle.AsymL1R2);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\TinyRoads\Oneway1LParkingBicycle\Textures\Ground_Segment__MainTex.png",
                            @"Roads\TinyRoads\Oneway1LParkingBicycle\Textures\Ground_Segment__APRMap.png"),
                        new LODTextureSet(
                            @"Roads\TinyRoads\Oneway1LParkingBicycle\Textures\Ground_Segment_LOD__MainTex.png",
                            @"Roads\TinyRoads\Oneway1LParkingBicycle\Textures\Ground_Segment_LOD__APRMap.png",
                            @"Roads\TinyRoads\Oneway1LParkingBicycle\Textures\Ground_LOD__XYSMap.png"));

                    for (int i = 0; i < info.m_nodes.Count(); i++)
                    {
                        if (info.m_nodes[i].m_flagsForbidden == NetNode.Flags.Transition)
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\TinyRoads\Oneway1LParkingBicycle\Textures\Ground_Node__MainTex.png",
                                     @"Roads\TinyRoads\Oneway1LParkingBicycle\Textures\Ground_Node__APRMap.png"),
                                new LODTextureSet(
                                    @"Roads\TinyRoads\Oneway1LParkingBicycle\Textures\Ground_Node_LOD__MainTex.png",
                                    @"Roads\TinyRoads\Oneway1LParkingBicycle\Textures\Ground_Node_LOD__APRMap.png",
                                    @"Roads\TinyRoads\Oneway1LParkingBicycle\Textures\Ground_LOD__XYSMap.png"));
                        }
                    }
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = true;
            info.m_halfWidth = 4f;
            info.m_pavementWidth = 1f;
            info.m_surfaceLevel = 0;
            info.m_class = roadInfo.m_class.Clone("NExt1LOnewayWithParkingBicycle");
            info.m_class.m_level = (ItemClass.Level)5; //New level
      
            var laneWidth = 2.5f;
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = false,
                LaneWidth = laneWidth,
                //LanesToAdd = 1,
                SpeedLimit = 0.6f,
                BusStopOffset = 0f,
                PedLaneOffset = -0.25f,
                PedPropOffsetX = 2.75f,
              //  LanePositionOffst = -2
            });
            
              var parkLane = info.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.Parking).CloneWithoutStops();
              parkLane.m_width = 2f;
              parkLane.m_position = -2.25f; //1.5
              parkLane.m_verticalOffset = 0.1f;
              info.m_lanes = info.m_lanes
                  .Where(l => l.m_laneType != NetInfo.LaneType.Parking)
                  .ToArray();
            parkLane.m_stopType = VehicleInfo.VehicleType.None;
            info.SetupNewSpeedLimitProps(30, 40);
            var carLine = info.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.Vehicle);
            carLine.m_position = 0.5f;
            carLine.m_verticalOffset = 0.1f;
            carLine.m_direction = NetInfo.Direction.Forward;
            var tempProps = carLine.m_laneProps.m_props.ToList();
            tempProps.RemoveProps("arrow");
            carLine.m_laneProps.m_props = tempProps.ToArray();
            carLine.m_stopType = VehicleInfo.VehicleType.Car;

            var bikelane = info.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.Vehicle);
            bikelane.m_width = 1;
            bikelane.m_direction = NetInfo.Direction.Forward;
            bikelane.m_speedLimit = 0.8f;
            bikelane.m_stopType = VehicleInfo.VehicleType.None;
            bikelane.m_vehicleType = VehicleInfo.VehicleType.Bicycle;
            bikelane.m_position = 2.4f;
            bikelane.m_verticalOffset = 0.1f;
            bikelane.SetBikeLaneProps();
            var pedLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();
            for (int i = 0; i < pedLanes.Count(); i++)
            {
                pedLanes[i].m_verticalOffset = 0.1f;
              //  pedLanes[i].m_stopType = VehicleInfo.VehicleType.None;
            }
            pedLanes[1].m_stopType = VehicleInfo.VehicleType.None;
            var tempLanes = new List<NetInfo.Lane>();
            tempLanes.Add(carLine);
            tempLanes.Add(bikelane);
            tempLanes.Add(parkLane);
            tempLanes.AddRange(pedLanes);
            info.m_lanes = tempLanes.ToArray();

            // left traffic light
            var newLeftTrafficLight = Prefabs.Find<PropInfo>("Traffic Light 01", false);
            var oldLeftTrafficLight = Prefabs.Find<PropInfo>("Traffic Light 02 Mirror", false);

            if (newLeftTrafficLight == null || oldLeftTrafficLight == null)
            {
                return;
            }

            info.ReplaceProps(newLeftTrafficLight, oldLeftTrafficLight);

            var originPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (playerNetAI != null && originPlayerNetAI != null)
            {
                playerNetAI.m_constructionCost = originPlayerNetAI.m_constructionCost * 2 / 3;
                playerNetAI.m_maintenanceCost = originPlayerNetAI.m_maintenanceCost * 2 / 3;
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();
            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = false;
            }
        }

        public void LateBuildUp(NetInfo info, NetInfoVersion version)
        {
            //var RoadPlanter1Name = "RoadPlanter1";
            //var RoadPlanter1 = PrefabCollection<PropInfo>.FindLoaded($"{Tools.PackageName(RoadPlanter1Name)}.{RoadPlanter1Name}_Data");
            //var pedLanes = info.m_lanes.Where(pl => pl.m_laneType == NetInfo.LaneType.Pedestrian).ToArray();
            //for (var i = 0; i < pedLanes.Length; i++)
            //{
            //    var planterProp = new NetLaneProps.Prop();
            //    planterProp.m_prop = RoadPlanter1;
            //    planterProp.m_finalProp = RoadPlanter1;
            //    planterProp.m_repeatDistance = 30;
            //    planterProp.m_probability = 100;
            //    planterProp.m_position = new UnityEngine.Vector3(((i * 2) - 1), 0.05f, -0.15f);
            //    var tempProps = pedLanes[i].m_laneProps.m_props.ToList();
            //    tempProps.Add(planterProp);
            //    pedLanes[i].m_laneProps.m_props = tempProps.ToArray();
            //}
        }
    }
}