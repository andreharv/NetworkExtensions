using Transit.Framework.UI.Infos;

namespace Transit.Addon.TM.UI.Toolbar.RoadEditor
{
    public class RoadEditorMenuMainCategoryInfo : IMenuCategoryInfo
    {
        public const string NAME = "RoadEditorMainCategory";

        public string Name { get { return NAME; } }
        public string DisplayName { get { return NAME; } }
        public int Order { get { return 10; } }
        public GeneratedGroupPanel.GroupFilter? Group { get { return null; } }
        public ItemClass.Service? Service { get { return null; } }
    }
}
