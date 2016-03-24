using System.Collections.Generic;
using Transit.Addon.TM.Tools.LaneRouting;
using Transit.Framework.Builders;

namespace Transit.Addon.TM.UI.Toolbar.RoadEditor
{
    public class RoadEditorMenuMainCategoryBuilder : IMenuCategoryBuilder
    {
        public const string NAME = "RoadEditorMainCategory";

        public string Name { get { return NAME; } }
        public string DisplayName { get { return NAME; } }
        public int Order { get { return 10; } }
        public GeneratedGroupPanel.GroupFilter? Group { get { return null; } }
        public ItemClass.Service? Service { get { return null; } }
    }
}
