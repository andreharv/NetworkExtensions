using System;
using System.Collections.Generic;
using Transit.Framework.UI.Toolbar.Items;
using Transit.Framework.UI.Toolbar.Menus;

namespace Transit.Addon.TM.UI.Toolbar.RoadEditor
{
    public class RoadEditorToolbarItemInfo : IToolbarMenuItemInfo
    {
        public const string NAME = "RoadEditor";

        public string Name { get { return NAME; } }
        public int Order { get { return 11; } }
        public Type PanelType { get { return typeof(RoadEditorMainPanel); } }
    }

    public class RoadEditorMainPanel : CustomGroupPanelBase
    {
        protected override void Initialize()
        {
            SpawnCategory(typeof(RoadEditorMainCategoryPanel), RoadEditorMainCategoryInfo.NAME, null, "SubBar", null, true);
        }
    }
}
