using System;
using Transit.Framework.UI.Toolbar.Items;
using Transit.Framework.UI.Toolbar.Menus;

namespace Transit.Addon.TM.UI.Toolbar.RoadEditor
{
    public class RoadEditorToolbarItemInfo : IToolbarMenuItemInfo
    {
        public const string NAME = "RoadEditor";

        public string Name { get { return NAME; } }
        public int Order { get { return 11; } }
        public Type PanelType { get { return typeof(RoadEditorToolbarPanel); } }
    }

    public class RoadEditorToolbarPanel : CustomGroupPanelBase
    {
        protected override void Initialize()
        {
            //var options = ToolModuleV3.TrafficToolsOptions;

            //if (options.IsFlagSet(ToolModuleV3.ModOptions.LaneRoutingTool))
            {
                SpawnCategory<RoadEditorMainCategoryPanel>("RoadEditorMain", null, "SubBar", null, true);
            }
        }
    }
}
