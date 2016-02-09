using System;
using ColossalFramework;
using Transit.Framework.UI;
using Transit.Framework.UI.Toolbar.Items;

namespace Transit.Addon.TrafficTools.Menus.RoadEditor
{
    public class RoadEditorToolbarItemInfo : IToolbarMenuItemInfo
    {
        public const string NAME = "RoadEditor";

        public string Name { get { return NAME; } }
        public int Order { get { return 11; } }
        public Type PanelType { get { return typeof(RoadEditorGroupPanel); } }
    }

    public class RoadEditorGroupPanel : CustomGroupPanelBase
    {
        protected override void Initialize()
        {
            var options = TrafficToolsModule.TrafficToolsOptions;

            if (options.IsFlagSet(TrafficToolsModule.ModOptions.LaneRoutingTool) ||
                options.IsFlagSet(TrafficToolsModule.ModOptions.TrafficLightsTool))
            {
                SpawnCategory<IntersectionEditorsPanel>("IntersectionEditors", null, "SubBar", null, true);
            }

            if (options.IsFlagSet(TrafficToolsModule.ModOptions.LaneRestrictorTool))
            {
                SpawnCategory<SegmentEditorsPanel>("SegmentEditors", null, "SubBar", null, true);
            }
        }
    }
}
