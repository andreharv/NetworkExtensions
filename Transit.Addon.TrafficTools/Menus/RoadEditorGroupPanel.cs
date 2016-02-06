using System;
using Transit.Framework.UI;
using Transit.Framework.UI.Toolbar.Items;

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

            //if ((options & TrafficToolsModule.ModOptions.LaneRestrictorTool) != 0)
            //{
            //    UIButton button = AddMode<LaneRestrictorTool>("Lane Restrictor Tool");
            //    SpawnMenuEntry("Lane Restrictor Tool", typeof(LaneVehicleRestrictorSubMenu), "LaneVehicleRestrictions", null, "SubBar", null, true);
            //    SpawnMenuEntry("Lane Restrictor Tool", typeof(LaneSpeedRestrictorSubMenu), "LaneSpeedRestrictions", null, "SubBar", null, true);

            //    Texture2D tex = AssetManager.instance.GetTexture(@"Tools\RCT\UI\laneRestrictor.png", TextureType.UI);
            //    tex.name = "laneRestrictor";
            //    textures.Add(tex);

            //    button.disabledFgSprite = button.pressedFgSprite = button.hoveredFgSprite = button.focusedFgSprite = button.normalFgSprite = "laneRestrictor";
            //    buttons.Add(button);
            //}

            //if ((options & TrafficToolsModule.ModOptions.TrafficLightsTool) != 0)
            //{
            //    UIButton button = AddMode<TrafficLightsTool>("Traffic Lights Tool");
            //    SpawnMenuEntry("Traffic Lights Tool", typeof(TrafficLightsSubMenu), "TrafficLightsTool", null, "SubBar", null, true);

            //    Texture2D tex = AssetManager.instance.GetTexture(@"Tools\RCT\UI\trafficLights.png", TextureType.UI);
            //    tex.name = "trafficLights";
            //    textures.Add(tex);

            //    button.disabledFgSprite = button.pressedFgSprite = button.hoveredFgSprite = button.focusedFgSprite = button.normalFgSprite = "trafficLights";
            //    buttons.Add(button);
            //}
        }
    }
}
