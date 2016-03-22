using System.Collections.Generic;
using ColossalFramework.UI;
using Transit.Framework;
using Transit.Framework.Builders;
using UnityEngine;

namespace Transit.Addon.TM.UI.Toolbar.RoadEditor.Textures
{
    public class ToolsAtlasBuilder : IAtlasBuilder
    {
        public IEnumerable<string> Keys
        {
            get
            {
                yield return "LaneRoutingTool";
                yield return "LaneRestrictorTool";
                yield return "TrafficLightsTool";
            }
        }

        public UITextureAtlas Build()
        {
            var thumbnailAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            thumbnailAtlas.padding = 0;
            thumbnailAtlas.name = "Tools";

            var shader = Shader.Find("UI/Default UI Shader");
            if (shader != null) thumbnailAtlas.material = new Material(shader);

            const string PATH = @"UI\Toolbar\RoadEditor\Textures\Tools.png";

            const string BG = "OptionBase";
            var versions = new[] { "", "Focused", "Hovered", "Pressed", "Disabled" };

            var texture = AssetManager.instance.GetTexture(PATH, TextureType.UI);
            texture.FixTransparency();

            thumbnailAtlas.material.mainTexture = texture;

            var x = 1;
            var y = 1;

            const int TEXTURE_W = 182;
            const int TEXTURE_H = 75;



            // Base -------------------------------------------------------------------------------
            const int BG_ICON_W = 36;
            const int BG_ICON_H = 36;

            foreach (var t in versions)
            {
                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = string.Format(BG + "{0}", t),
                    region = new Rect(
                        (float)(x) / TEXTURE_W,
                        (float)(y) / TEXTURE_H,
                        (float)(BG_ICON_W) / TEXTURE_W,
                        (float)(BG_ICON_H) / TEXTURE_H),
                    texture = new Texture2D(BG_ICON_W, BG_ICON_H, TextureFormat.ARGB32, false)
                };

                thumbnailAtlas.AddSprite(sprite);

                x += BG_ICON_W;
            }
            y += BG_ICON_H + 1;



            // Button Icons -----------------------------------------------------------------------
            const int FG_ICON_W = 36;
            const int FG_ICON_H = 36;
            x = 1;

            foreach (var name in Keys)
            {
                foreach (var t in versions)
                {
                    var sprite = new UITextureAtlas.SpriteInfo
                    {
                        name = string.Format(name + "{0}", t),
                        region = new Rect(
                            (float)(x) / TEXTURE_W,
                            (float)(y) / TEXTURE_H,
                            (float)(FG_ICON_W) / TEXTURE_W,
                            (float)(FG_ICON_H) / TEXTURE_H),
                        texture = new Texture2D(FG_ICON_W, FG_ICON_H, TextureFormat.ARGB32, false)
                    };

                    thumbnailAtlas.AddSprite(sprite);
                }

                x += FG_ICON_W;
            }

            return thumbnailAtlas;
        }
    }
}
