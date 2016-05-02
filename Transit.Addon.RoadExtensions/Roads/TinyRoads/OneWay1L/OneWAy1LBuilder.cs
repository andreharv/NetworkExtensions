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

namespace Transit.Addon.RoadExtensions.Roads.TinyRoads.OneWay1L
{
    public class OneWay1LBuilder : Activable, INetInfoBuilderPart
    {
        public const string NAME = "One-Lane Oneway";

        public int Order { get { return 1; } }
        public int UIOrder { get { return 10; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public string Name { get { return NAME; } }
        public string DisplayName { get { return NAME; } }
        public string CodeName { get { return "Oneway1L"; } }
        public string Description { get { return "A one-lane, oneway road suitable for neighborhood traffic."; } }
        public string ShortDescription { get { return "Zoneable, neighborhood traffic"; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_TINY; } }

        public string ThumbnailsPath { get { return @"Roads\TinyRoads\OneWay1L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\TinyRoads\OneWay1L\infotooltip.png"; } }

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
            info.Setup8m1p5mSWMesh(version, AsymLaneType.L1R2);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\TinyRoads\OneWay1L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\TinyRoads\OneWay1L\Textures\Ground_Segment__APRMap.png"),
                        new LODTextureSet(
                            @"Roads\TinyRoads\OneWay1L\Textures\Ground_Segment_LOD__MainTex.png",
                            @"Roads\TinyRoads\OneWay1L\Textures\Ground_Segment_LOD__APRMap.png",
                            @"Roads\TinyRoads\OneWay1L\Textures\Ground_LOD__XYSMap.png"));

                    for (int i = 0; i < info.m_nodes.Count(); i++)
                    {
                        if (info.m_nodes[i].m_flagsForbidden == NetNode.Flags.Transition)
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\TinyRoads\OneWay1L\Textures\Ground_Node__MainTex.png",
                                     @"Roads\TinyRoads\OneWay1L\Textures\Ground_Node__APRMap.png"),
                                new LODTextureSet(
                                    @"Roads\TinyRoads\OneWay1L\Textures\Ground_Node_LOD__MainTex.png",
                                    @"Roads\TinyRoads\OneWay1L\Textures\Ground_Node_LOD__APRMap.png",
                                    @"Roads\TinyRoads\OneWay1L\Textures\Ground_LOD__XYSMap.png"));
                        }
                        else if (info.m_nodes[i].m_flagsRequired == NetNode.Flags.Transition)
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\TinyRoads\OneWay1L\Textures\Ground_Trans__MainTex.png",
                                     @"Roads\TinyRoads\OneWay1L\Textures\Ground_Trans__APRMap.png"),
                                new LODTextureSet(
                                    @"Roads\TinyRoads\OneWay1L\Textures\Ground_Trans_LOD__MainTex.png",
                                    @"Roads\TinyRoads\OneWay1L\Textures\Ground_Trans_LOD__APRMap.png",
                                    @"Roads\TinyRoads\OneWay1L\Textures\Ground_LOD__XYSMap.png"));
                        }
                    }
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = true;
            info.m_halfWidth = 4f;
            info.m_pavementWidth = 1.5f;
            info.m_surfaceLevel = 0;
            info.m_class = roadInfo.m_class.Clone("NExt1LOneway");
            var parkLane = info.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.Parking).CloneWithoutStops();
            info.m_class.m_level = (ItemClass.Level)5; //New level
            parkLane.m_width = 2f;
            info.m_lanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.Parking)
                .ToArray();

            var laneWidth = 3f;
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = false,
                LaneWidth = laneWidth,
                LanesToAdd = -1,
                SpeedLimit = 0.6f,
                BusStopOffset = 0f,
                PedLaneOffset = -0.75f,
                PedPropOffsetX = 2.25f
            });
            info.SetupNewSpeedLimitProps(30, 40);
            info.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.Vehicle).m_position = (info.m_halfWidth - info.m_pavementWidth - (0.5f * laneWidth)) * -1;
            parkLane.m_position = (info.m_halfWidth - info.m_pavementWidth - (0.5f * parkLane.m_width));
            var tempLanes = new List<NetInfo.Lane>();
            tempLanes.AddRange(info.m_lanes);
            tempLanes.Add(parkLane);
            info.m_lanes = tempLanes.ToArray();

            var pedLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
            var roadLanes = info.m_lanes.Where(l => l.m_laneType != NetInfo.LaneType.Pedestrian && l.m_laneType != NetInfo.LaneType.None);

            foreach (var pedLane in pedLanes)
            {
                pedLane.m_verticalOffset = 0.25f;
            }

            foreach (var roadLane in roadLanes)
            {
                roadLane.m_verticalOffset = 0.1f;
            }

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
                playerNetAI.m_constructionCost = originPlayerNetAI.m_constructionCost * 1 / 2;
                playerNetAI.m_maintenanceCost = originPlayerNetAI.m_maintenanceCost * 1 / 2;
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();
            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = false;
            }
        }
    }
}