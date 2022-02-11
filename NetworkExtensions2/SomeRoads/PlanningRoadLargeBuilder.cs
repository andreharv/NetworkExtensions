// TODO Later

//using System;
//using System.Linq;
//using Transit.Framework;

//namespace Transit.Addon.RoadExtensions.SomeRoads
//{
//    public class PlanningRoadLargeBuilder : INetInfoBuilder
//    {
//        public int Priority { get { return 1002; } }

//        public string PrefabName  { get { return NetInfos.VanillaNetInfos.ROAD_6L; } }
//        public string Name        { get { return "Planning Road (Large)"; } }
//        public string Description { get { return "A non-functional road for planning road layouts. Can be upgraded to actual roads."; } }
//        public string UICategory  { get { return "SomeRoads"; } }

//        public string ThumbnailsPath { get { return null; } }
//        public string InfoTooltipPath { get { return null; } }

//        public NetInfoVersion SupportedVersions
//        {
//            get { return NetInfoVersion.Ground; }
//        }

//        public string GetPrefabName(NetInfoVersion version)
//        {
//            switch (version)
//            {
//                case NetInfoVersion.Ground:
//                    return PrefabName;
//                default:
//                    throw new NotImplementedException();
//            }
//        }

//        public void BuildUp(NetInfo info, NetInfoVersion version)
//        {
//            info.SetSegmentsTexture(new TextureSet(@"SomeNetwork\Textures\planning_road_2_lanes.png"));
//            info.SetNodesTexture(new TextureSet(@"SomeNetwork\Textures\planning_road_2_lanes.png"));

//            info.m_lanes = new NetInfo.Lane[] { };
//            info.m_color.r = 0.4f;
//            info.m_color.g = 0.6f;
//            info.m_color.b = 0.8f;
//            info.m_createPavement = false;
//            info.m_createGravel = false;
//            info.m_createRuining = false;
//            info.m_hasParkingSpaces = false;
//            info.m_hasPedestrianLanes = false;

//            var ai = info.GetComponent<RoadAI>();
//            ai.m_constructionCost = 0;
//            ai.m_maintenanceCost = 0;
//            ai.m_noiseRadius = 0;
//            ai.m_noiseAccumulation = 0;
//            ai.m_enableZoning = false;
//        }
//    }
//}
