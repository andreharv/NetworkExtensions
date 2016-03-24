using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.UI.Toolbar.Roads
{
    public class TinyRoadsCategoryBuilder : IMenuCategoryBuilder
    {
        public const string NAME = "RoadsTiny";

        public string Name { get { return NAME; } }
        public string DisplayName { get { return "Tiny Roads"; } }
        public int Order { get { return 5; } }
        public GeneratedGroupPanel.GroupFilter? Group { get { return GeneratedGroupPanel.GroupFilter.Net; } }
        public ItemClass.Service? Service { get { return ItemClass.Service.Road; } }
    }

    public class SmallHeavyRoadsCategoryBuilder : IMenuCategoryBuilder
    {
        public const string NAME = "RoadsSmallHV";

        public string Name { get { return NAME; } }
        public string DisplayName { get { return "Small Heavy Roads"; } }
        public int Order { get { return 20; } }
        public GeneratedGroupPanel.GroupFilter? Group { get { return GeneratedGroupPanel.GroupFilter.Net; } }
        public ItemClass.Service? Service { get { return ItemClass.Service.Road; } }
    }

    public class BusRoadsCategoryBuilder : IMenuCategoryBuilder
    {
        public const string NAME = "RoadsBusways";

        public string Name { get { return NAME; } }
        public string DisplayName { get { return "Buslane Roads"; } }
        public int Order { get { return 65; } }
        public GeneratedGroupPanel.GroupFilter? Group { get { return GeneratedGroupPanel.GroupFilter.Net; } }
        public ItemClass.Service? Service { get { return ItemClass.Service.Road; } }
    }

    public class PedestriansRoadsCategoryBuilder : IMenuCategoryBuilder
    {
        public const string NAME = "RoadsPedestrians";

        public string Name { get { return NAME; } }
        public string DisplayName { get { return "Pedestrian Roads"; } }
        public int Order { get { return 75; } }
        public GeneratedGroupPanel.GroupFilter? Group { get { return GeneratedGroupPanel.GroupFilter.Net; } }
        public ItemClass.Service? Service { get { return ItemClass.Service.Road; } }
    }
}
