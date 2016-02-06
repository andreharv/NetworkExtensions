using System;
using ColossalFramework.UI;
using Transit.Framework;
using Transit.Framework.UI;
using Transit.Framework.UI.Toolbar.Items;
using UnityEngine;

namespace Transit.Addon.TrafficTools.Menus
{
    public class RoadEditorToolbarItemInfo : IToolbarMenuItemInfo
    {
        public const string NAME = "RoadEditor";

        public string Name { get { return NAME; } }
        public int Order { get { return 11; } }
        public Type PanelType { get { return typeof(RoadEditorGroupPanel); } }
    }

    public class RoadEditorGroupPanel : MultiModeGroupPanel
    {
        protected override void Initialize()
        {
            var options = TrafficToolsModule.TrafficToolsOptions;

            if ((options & TrafficToolsModule.ModOptions.LaneRoutingTool) != 0)
            {
                AddMode<LaneRoutingTool>("LaneRoutingTool");
                SpawnMenuEntry("LaneRoutingTool", typeof(LaneRoutingTemplatesSubMenu), "LaneRoutingTemplates", null, "SubBar", null, true);
            }

            if ((options & TrafficToolsModule.ModOptions.LaneRestrictorTool) != 0)
            {
                AddMode<LaneRestrictorTool>("LaneRestrictorTool");
                SpawnMenuEntry("LaneRestrictorTool", typeof(LaneVehicleRestrictorSubMenu), "LaneVehicleRestrictions", null, "SubBar", null, true);
                SpawnMenuEntry("LaneRestrictorTool", typeof(LaneSpeedRestrictorSubMenu), "LaneSpeedRestrictions", null, "SubBar", null, true);
            }

            if ((options & TrafficToolsModule.ModOptions.TrafficLightsTool) != 0)
            {
                AddMode<TrafficLightsTool>("TrafficLightsTool");
                SpawnMenuEntry("TrafficLightsTool", typeof(TrafficLightsSubMenu), "TrafficLightsTool", null, "SubBar", null, true);
            }
        }
    }
}
