using System;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.Roads.TinyRoads.OneWay1L
{
    public class OneWay1LBuilder : Activable, INetInfoBuilderPart
    {
        public const string NAME = "One-Lane Oneway Road";

        public int Order { get { return 1; } }
        public int UIOrder { get { return 5; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public string Name { get { return NAME; } }
        public string DisplayName { get { return NAME; } }
        public string CodeName { get { return "Oneway1L"; } }
        public string Description { get { return "A one-lane, oneway road suitable for neighborhood traffic."; } }
        public string ShortDescription { get { return "Oneway, No parking, zoneable, neighborhood traffic"; } }
        public string UICategory { get { return AdditionnalMenus.ROADS_TINY; } }

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
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L);

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup8m2mSWMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\TinyRoads\OneWay1L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\TinyRoads\OneWay1L\Textures\Ground_Segment__APRMap.png"),
                        new LODTexturesSet(
                            @"Roads\TinyRoads\OneWay1L\Textures\Ground_Segment_LOD__MainTex.png",
                            @"Roads\TinyRoads\OneWay1L\Textures\Ground_Segment_LOD__APRMap.png",
                            @"Roads\TinyRoads\OneWay1L\Textures\Ground_LOD__XYSMap.png"));

                    info.SetAllNodesTexture(
                                new TexturesSet
                                    (@"Roads\TinyRoads\OneWay1L\Textures\Ground_Node__MainTex.png",
                                     @"Roads\TinyRoads\OneWay1L\Textures\Ground_Node__APRMap.png"),
                                new LODTexturesSet(
                                    @"Roads\TinyRoads\OneWay1L\Textures\Ground_Node_LOD__MainTex.png",
                                    @"Roads\TinyRoads\OneWay1L\Textures\Ground_Node_LOD__APRMap.png",
                                    @"Roads\TinyRoads\OneWay1L\Textures\Ground_LOD__XYSMap.png"));
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_halfWidth = 4f;
            info.m_pavementWidth = 2f;
            info.m_class = roadInfo.m_class.Clone("NExt1LOneway");

            info.m_lanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.Parking)
                .ToArray();

            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                LaneWidth = 4f,
                LanesToAdd = -1,
                SpeedLimit = 0.6f,
                BusStopOffset = 0f,
                PedLaneOffset = -0.75f,
                PedPropOffsetX = 2.25f
            });
            info.SetupNewSpeedLimitProps(30, 40);
            //info.TrimArrowsProps();
            
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
