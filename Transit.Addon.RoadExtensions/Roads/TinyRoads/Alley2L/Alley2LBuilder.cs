using System;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.TinyRoads.Alley2L
{
    public class Alley2LBuilder : Activable, INetInfoBuilderPart
    {
        public const string NAME = "Two-Lane Alley";

        public int Order { get { return 0; } }
        public int UIOrder { get { return 5; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string Name { get { return NAME; } }
        public string DisplayName { get { return NAME; } }
        public string CodeName { get { return "Alley_2L"; } }
        public string Description { get { return "A two-lane, tight Alley suitable for neighborhood traffic."; } }
        public string ShortDescription { get { return "No parking, zoneable, neighborhood traffic"; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_TINY; } }

        public string ThumbnailsPath { get { return @"Roads\TinyRoads\Alley2L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\TinyRoads\Alley2L\infotooltip.png"; } }

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
            info.Setup8m1p5mSWMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\TinyRoads\Alley2L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\TinyRoads\Alley2L\Textures\Ground_Segment__APRMap.png"),
                        new LODTextureSet(
                            @"Roads\TinyRoads\Alley2L\Textures\Ground_Segment_LOD__MainTex.png",
                            @"Roads\TinyRoads\Alley2L\Textures\Ground_Segment_LOD__APRMap.png",
                            @"Roads\TinyRoads\Alley2L\Textures\Ground_LOD__XYSMap.png"));

                    for (int i = 0; i < info.m_nodes.Count(); i++)
                    {
                        if (info.m_nodes[i].m_flagsForbidden == NetNode.Flags.Transition)
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\TinyRoads\Alley2L\Textures\Ground_Node__MainTex.png",
                                     @"Roads\TinyRoads\Alley2L\Textures\Ground_Node__APRMap.png"),
                                new LODTextureSet(
                                    @"Roads\TinyRoads\Alley2L\Textures\Ground_Node_LOD__MainTex.png",
                                    @"Roads\TinyRoads\Alley2L\Textures\Ground_Node_LOD__APRMap.png",
                                    @"Roads\TinyRoads\Alley2L\Textures\Ground_LOD__XYSMap.png"));
                        }
                        else if (info.m_nodes[i].m_flagsRequired == NetNode.Flags.Transition)
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\TinyRoads\Alley2L\Textures\Ground_Trans__MainTex.png",
                                     @"Roads\TinyRoads\Alley2L\Textures\Ground_Trans__APRMap.png"),
                                new LODTextureSet(
                                    @"Roads\TinyRoads\Alley2L\Textures\Ground_Trans_LOD__MainTex.png",
                                    @"Roads\TinyRoads\Alley2L\Textures\Ground_Trans_LOD__APRMap.png",
                                    @"Roads\TinyRoads\Alley2L\Textures\Ground_LOD__XYSMap.png"));
                        }
                    }
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_halfWidth = 4f;
            info.m_pavementWidth = 1.5f;
            info.m_surfaceLevel = 0;
            info.m_class = roadInfo.m_class.Clone("NExt2LAlley");
            info.m_class.m_level = (ItemClass.Level)5; //New level
            info.m_lanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.Parking)
                .ToArray();

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

            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                LaneWidth = 2.5f,
                SpeedLimit = 0.6f,
                BusStopOffset = 0f,
                PedLaneOffset = -0.75f,
                PedPropOffsetX = 2.25f
            });
            info.SetupNewSpeedLimitProps(30, 40);
            info.TrimArrowsProps();
            
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
