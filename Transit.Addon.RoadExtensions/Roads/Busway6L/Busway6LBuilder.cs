using Transit.Addon.RoadExtensions.Menus;
using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.Busway6L
{
    public class Busway6LBuilder : NetInfoBuilderBase, INetInfoBuilder
    {
        public int Order { get { return 170; } }
        public int Priority { get { return 26; } }

        public string TemplatePrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "Large Busway"; } }
        public string DisplayName { get { return "Busway"; } }
        public string CodeName { get { return "BUSWAY_6L"; } }
        public string Description { get { return "A two-lane, two-way road suitable for buses only. Busway does not allow zoning next to it!"; } }
        public string UICategory { get { return AdditionnalMenus.ROADS_BUSWAYS; } }

        public string ThumbnailsPath { get { return null; } }
        public string InfoTooltipPath { get { return null; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
        }
    }
}
