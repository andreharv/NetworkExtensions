using Transit.Framework;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads
{
    public class ZonablePedestrianGravelBuilder : ZonablePedestrianBuilderBase, INetInfoBuilderPart
    {
        public int Order { get { return 300; } }
        public int UIOrder { get { return 10; } }

        public string Name { get { return "Zonable Pedestrian Gravel"; } }
        public string DisplayName { get { return "Zonable Pedestrian Gravel"; } }
        public string Description { get { return "Gravel roads allow pedestrians to walk fast and easy."; } }

        public string ThumbnailsPath { get { return @"Roads\PedestrianRoads\thumbnails_gravel.png"; } }
        public string InfoTooltipPath { get { return string.Empty; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public override void BuildUp(NetInfo info, NetInfoVersion version)
        {
            info.m_createGravel = true;
            info.m_createPavement = false;

            base.BuildUp(info, version);
        }
    }
}
