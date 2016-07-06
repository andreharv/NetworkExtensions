using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.SmallHeavyRoads.AsymRoad3L.AsymRoadL2R1
{
    public class AsymRoadL2R1Builder : AsymRoad3LBuilderBase, INetInfoBuilderPart
    {
        public int Order { get { return 9; } }
        public int UIOrder { get { return 50; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string Name { get { return "AsymRoadL2R1"; } }
        public string DisplayName { get { return "Asymmetrical Road: 2 Left / 1 Right"; } }
        public string Description { get { return "An asymmetrical road with two left lanes and one right lane.  Note, dragging this road backwards reverses its orientation."; } }
        public string ShortDescription { get { return "No parking, zoneable, low traffic"; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_SMALL_HV; } }

        public string ThumbnailsPath { get { return @"Roads\SmallHeavyRoads\AsymRoad3L\AsymRoadL2R1\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\SmallHeavyRoads\AsymRoad3L\AsymRoadL2R1\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public override LanesLayoutStyle LanesLayoutStyle
        {
            get { return LanesLayoutStyle.AsymL2R1; }
        }
    }
}
