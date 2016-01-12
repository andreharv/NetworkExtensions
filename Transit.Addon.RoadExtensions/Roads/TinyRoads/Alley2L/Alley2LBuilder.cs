using System;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.Roads.TinyRoads.Alley2L
{
    public class Alley2LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 0; } }
        public int UIOrder { get { return 5; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string Name { get { return "Two-Lane Alley"; } }
        public string DisplayName { get { return "Two-Lane Alley"; } }
        public string CodeName { get { return "Alley_2L"; } }
        public string Description { get { return "A two-lane, tight Alley suitable for neighborhood traffic. This road is not zonable."; } }
        public string ShortDescription { get { return "No parking, not zoneable, neighborhood traffic"; } }
        public string UICategory { get { return AdditionnalMenus.ROADS_TINY; } }

        public string ThumbnailsPath { get { return string.Empty; } }
        public string InfoTooltipPath { get { return string.Empty; } }

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
            if (version == NetInfoVersion.Ground)
            {
                info.m_surfaceLevel = 0;

                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.SetMeshes
                    (@"Roads\TinyRoads\Alley2L\Meshes\Ground.obj",
                     @"Roads\TinyRoads\Alley2L\Meshes\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"Roads\TinyRoads\Alley2L\Meshes\Ground.obj",
                     @"Roads\TinyRoads\Alley2L\Meshes\Ground_Node_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0 };
            }

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Roads\TinyRoads\Alley2L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\TinyRoads\Alley2L\Textures\Ground_Segment__AlphaMap.png"));
                    //info.SetNodesTexture(
                    //    new TexturesSet
                    //        (@"Roads\TinyRoads\Alley2L\Textures\Ground_Node__MainTex.png",
                    //         @"Roads\TinyRoads\Alley2L\Textures\Ground_Node__AlphaMap.png"));
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_halfWidth = 4f;
            info.m_pavementWidth = 2f;

            info.m_lanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.Parking)
                .ToArray();

            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                LaneWidth = 2f,
                SpeedLimit = 0.6f,
                BusStopOffset = 0f,
                PedPropOffsetX = 1.5f
            });
            info.SetupNewSpeedLimitProps(40, 30);

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
