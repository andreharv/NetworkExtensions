using ICities;
using System;
using Transit.Framework.Modularity;

namespace Transit.Addon.TrafficTools
{
    public partial class TrafficToolsModule : ModuleBase
    {
        [Flags]
        public enum Options
        {
            None = 0,

            LaneRoutingTool         = 1 << 27,
            LaneRestrictorTool      = 1 << 28,
            //LanePrioritiesTool      = 1 << 29,
            TrafficLightsTool       = 1 << 30
        }

        private static Options s_options = Options.LaneRoutingTool | Options.LaneRestrictorTool/* | Options.TrafficLightsTool*/;
        public static Options TrafficToolsOptions { get { return s_options; } }

        public override void OnSettingsUI(UIHelperBase helper)
        {
            //helper.AddCheckbox(
            //    "Lane Routing Tool",
            //    "Allows you to customize entry and exit points in junctions.",
            //    (TrafficToolsOptions & Options.LaneRoutingTool) != 0,
            //    OnCheckboxChanged,
            //    true,
            //    Options.LaneRoutingTool);

            //helper.AddCheckbox(
            //    "Lane Restrictor Tool",
            //    "Allows you to restrict vehicle and speed usage on lanes.",
            //    (TrafficToolsOptions & Options.LaneRestrictorTool) != 0,
            //    OnCheckboxChanged,
            //    true,
            //    Options.LaneRestrictorTool);

            //helper.AddCheckbox(
            //    "Traffic Lights Tool",
            //    "Allows you to toggle and setup sequences at traffic lights.",
            //    (TrafficToolsOptions & Options.TrafficLightsTool) != 0,
            //    OnCheckboxChanged,
            //    true,
            //    Options.TrafficLightsTool);
        }
    }
}
