using System;
using ColossalFramework;
using Transit.Addon.TrafficTools.RoadEditor.IntersectionEditors;
using Transit.Addon.TrafficTools.RoadEditor.LaneRestrictions;
using Transit.Addon.TrafficTools.RoadEditor.SegmentEditors;
using Transit.Framework.UI;
using Transit.Framework.UI.Toolbar.Items;

namespace Transit.Addon.TrafficTools.RoadEditor
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

                //AddMode<LaneRoutingTool>("LaneRoutingTool");
                //SpawnCategory("IntersectionEditors", typeof(LaneRoutingTemplatesSubMenu), "LaneRoutingTemplates", null, "SubBar", null, true);
            }

            if (options.IsFlagSet(TrafficToolsModule.ModOptions.LaneRestrictorTool))
            {
                SpawnCategory<SegmentEditorsPanel>("SegmentEditors", null, "SubBar", null, true);

                //AddMode<LaneRoutingTool>("LaneRoutingTool");
                //SpawnCategory("IntersectionEditors", typeof(LaneRoutingTemplatesSubMenu), "LaneRoutingTemplates", null, "SubBar", null, true);
            }

            //if ((options & TrafficToolsModule.ModOptions.LaneRestrictorTool) != 0)
            //{
            //    //AddMode<LaneRestrictorTool>("LaneRestrictorTool");
            //    SpawnCategory("LaneRestrictorTool", typeof(LaneVehicleRestrictorSubMenu), "LaneVehicleRestrictions", null, "SubBar", null, true);
            //    SpawnCategory("LaneRestrictorTool", typeof(LaneSpeedRestrictorSubMenu), "LaneSpeedRestrictions", null, "SubBar", null, true);
            //}

            //if ((options & TrafficToolsModule.ModOptions.TrafficLightsTool) != 0)
            //{
            //    //AddMode<TrafficLightsTool>("TrafficLightsTool");
            //    SpawnCategory("TrafficLightsTool", typeof(TrafficLightsSubMenu), "TrafficLightsTool", null, "SubBar", null, true);
            //}
        }
    }
}
