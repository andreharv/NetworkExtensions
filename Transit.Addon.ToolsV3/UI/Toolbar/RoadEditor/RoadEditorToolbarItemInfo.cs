using System;
using ColossalFramework;
using Transit.Framework.UI.Toolbar.Menus;
using Transit.Framework.UI.Toolbar.Items;

namespace Transit.Addon.ToolsV3.UI.Toolbar.RoadEditor
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
            var options = ToolModuleV3.TrafficToolsOptions;

            if (options.IsFlagSet(ToolModuleV3.ModOptions.LaneRoutingTool) ||
                options.IsFlagSet(ToolModuleV3.ModOptions.TrafficLightsTool))
            {
                SpawnCategory<RoadEditorMainPanel>("IntersectionEditors", null, "SubBar", null, true);
            }
        }
    }
}
