using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transit.Framework;
using Transit.Framework.UI;
using UnityEngine;

namespace Transit.Addon.TrafficTools
{
    public class RoadCustomizerToolsMenu : ToolbarMenu
    {
        protected override void Initialize()
        {
            TrafficToolsModule.Options options = TrafficToolsModule.TrafficToolsOptions;

            List<UIButton> buttons = new List<UIButton>();
            List<Texture2D> textures = new List<Texture2D>();
            if ((options & TrafficToolsModule.Options.LaneRoutingTool) != 0)
            {
                UIButton button = AddMode<LaneRoutingTool>("Lane Routing Tool");
                SpawnMenuEntry("Lane Routing Tool", typeof(LaneRoutingTemplatesSubMenu), "LaneRoutingTemplates", null, "SubBar", null, true);

                Texture2D tex = AssetManager.instance.GetTexture(@"Tools\RCT\UI\laneRouting.png", TextureType.UI);
                tex.name = "laneRouting";
                textures.Add(tex);

                button.disabledFgSprite = button.pressedFgSprite = button.hoveredFgSprite = button.focusedFgSprite = button.normalFgSprite = "laneRouting";
                buttons.Add(button);
            }

            if ((options & TrafficToolsModule.Options.LaneRestrictorTool) != 0)
            {
                UIButton button = AddMode<LaneRestrictorTool>("Lane Restrictor Tool");
                SpawnMenuEntry("Lane Restrictor Tool", typeof(LaneVehicleRestrictorSubMenu), "LaneVehicleRestrictions", null, "SubBar", null, true);
                SpawnMenuEntry("Lane Restrictor Tool", typeof(LaneSpeedRestrictorSubMenu), "LaneSpeedRestrictions", null, "SubBar", null, true);

                Texture2D tex = AssetManager.instance.GetTexture(@"Tools\RCT\UI\laneRestrictor.png", TextureType.UI);
                tex.name = "laneRestrictor";
                textures.Add(tex);

                button.disabledFgSprite = button.pressedFgSprite = button.hoveredFgSprite = button.focusedFgSprite = button.normalFgSprite = "laneRestrictor";
                buttons.Add(button);
            }

            if ((options & TrafficToolsModule.Options.TrafficLightsTool) != 0)
            {
                UIButton button = AddMode<TrafficLightsTool>("Traffic Lights Tool");
                SpawnMenuEntry("Traffic Lights Tool", typeof(TrafficLightsSubMenu), "TrafficLightsTool", null, "SubBar", null, true);

                Texture2D tex = AssetManager.instance.GetTexture(@"Tools\RCT\UI\trafficLights.png", TextureType.UI);
                tex.name = "trafficLights";
                textures.Add(tex);

                button.disabledFgSprite = button.pressedFgSprite = button.hoveredFgSprite = button.focusedFgSprite = button.normalFgSprite = "trafficLights";
                buttons.Add(button);
            }

            // Thumbnails
            if (buttons.Count == 0)
                return;

            UIButton btn = buttons[0];
            textures.Add(btn.atlas.sprites.Find(s => s.name == "OptionBase").texture);
            textures.Add(btn.atlas.sprites.Find(s => s.name == "OptionBaseDisabled").texture);
            textures.Add(btn.atlas.sprites.Find(s => s.name == "OptionBaseFocused").texture);
            textures.Add(btn.atlas.sprites.Find(s => s.name == "OptionBaseHovered").texture);
            textures.Add(btn.atlas.sprites.Find(s => s.name == "OptionBasePressed").texture);

            UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            atlas.padding = 0;
            atlas.name = "RCT";

            var shader = Shader.Find("UI/Default UI Shader");
            if (shader != null)
                atlas.material = new Material(shader);

            atlas.material.mainTexture = new Texture2D(1, 1);
            atlas.AddTextures(textures.ToArray());

            foreach (var b in buttons)
            {
                b.atlas = atlas;
            }
        }
    }
}
