//using Transit.Framework.Modularity;

//namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads
//{
//    public class ZonablePedestrianGravelBuilder : ZonablePedestrianBuilderBase, INetInfoBuilder
//    {
//        public int Order { get { return 300; } }
//        public int Priority { get { return 10; } }

//        public string Name { get { return "Zonable Pedestrian Gravel"; } }
//        public string DisplayName { get { return "Zonable Pedestrian Gravel"; } }
//        public string CodeName { get { return "Z_PED_GRAVEL"; } }
//        public string Description { get { return "Gravel roads allow pedestrians to walk fast and easy."; } }

//        public string ThumbnailsPath    { get { return @"Roads\PedestrianRoads\thumbnails_gravel.png"; } }
//        public string InfoTooltipPath   { get { return @"Roads\PedestrianRoads\infotooltip.png"; } }

//        public NetInfoVersion SupportedVersions
//        {
//            get { return NetInfoVersion.Ground; }
//        }

//        public override void BuildUp(NetInfo info, NetInfoVersion version)
//        {
//            info.m_createGravel = true;
//            info.m_createPavement = false;

//            base.BuildUp(info, version);
//        }
//    }
//}
