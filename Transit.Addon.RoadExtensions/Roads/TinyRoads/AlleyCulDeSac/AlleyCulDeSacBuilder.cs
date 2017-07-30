using System;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.TinyRoads.AlleyCulDeSac
{
    public class AlleyCulDeSacBuilder : Activable, INetInfoBuilderPart
    {
        public const string NAME = "Tiny Cul-De-Sac";

        public int Order { get { return 1; } }
        public int UIOrder { get { return 10; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public string Name { get { return NAME; } }
        public string DisplayName { get { return NAME; } }
        public string CodeName { get { return "AlleyCulDeSac"; } }
        public string Description { get { return "A one-lane, oneway road suitable for neighborhood traffic."; } }
        public string ShortDescription { get { return "No parking, zoneable, neighborhood traffic"; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_TINY; } }

        public string ThumbnailsPath { get { return @"Roads\TinyRoads\AlleyCulDeSac\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\TinyRoads\AlleyCulDeSac\infotooltip.png"; } }

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
            info.Setup8m1p5mSW1SMesh(version, LanesLayoutStyle.AsymL1R2);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment__MainTex.png",
                            @"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment_LOD__MainTex.png",
                            @"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment_LOD__APRMap.png",
                            @"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_LOD__XYSMap.png"));

                    info.SetAllNodesTexture(
                        new TextureSet
                           (@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment__MainTex.png",
                            @"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment_LOD__MainTex.png",
                            @"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment_LOD__APRMap.png",
                            @"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_LOD__XYSMap.png"));
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = true;
            info.m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
            info.m_nodeConnectGroups = NetInfo.ConnectGroup.NarrowTram;
            info.m_halfWidth = 4f;
            info.m_pavementWidth = 2f;
            info.m_class = roadInfo.m_class.Clone("NExt1LOnewayX");
            info.m_class.m_level = ItemClass.Level.Level2;

            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = false,
                LaneWidth = 4f,
                LanesToAdd = -1,
                SpeedLimit = 0.6f,
                BusStopOffset = 0f,
                PedLaneOffset = -0.75f,
                PedPropOffsetX = 2.25f
            });
            info.SetupNewSpeedLimitProps(30, 40);

            for (var i = 0; i < info.m_lanes.Count(); i++)
            {
                if (info.m_lanes[i]?.m_laneProps?.m_props != null)
                {
                    for (var j = 0; j < info.m_lanes[i].m_laneProps.m_props.Count(); j++)
                    {
                        if (info.m_lanes[i].m_laneProps.m_props[j] != null)
                        {
                            info.m_lanes[i].m_laneProps.m_props[j].m_probability = 0;
                        }
                    }
                }

            }

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
