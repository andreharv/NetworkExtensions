using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.SmallHeavyRoads.AsymRoad3L.AsymRoadL1R2
{
    public class AsymRoadL1R2Builder : AsymRoad3LBuilderBase, INetInfoBuilderPart, INetInfoInvertableBuilder
    {
        public int Order { get { return 8; } }
        public int UIOrder { get { return 40; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string Name { get { return "AsymRoadL1R2"; } }
        public string DisplayName { get { return "Asymmetrical Road: 1 Left / 2 Right"; } }
        public string Description { get { return "An asymmetrical road with one left lane and two right lanes.  Note, dragging this road backwards reverses its orientation."; } }
        public string ShortDescription { get { return "No parking, zoneable, low traffic"; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_SMALL_HV; } }

        public string ThumbnailsPath { get { return @"Roads\SmallHeavyRoads\AsymRoad3L\AsymRoadL1R2\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\SmallHeavyRoads\AsymRoad3L\AsymRoadL1R2\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public override LanesLayoutStyle LanesLayoutStyle
        {
            get { return LanesLayoutStyle.AsymL1R2; }
        }
    }
}
