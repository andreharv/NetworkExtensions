using ColossalFramework.UI;
using System.Collections.Generic;
using Transit.Framework;
using Transit.Framework.Builders;
using UnityEngine;

namespace Transit.Addon.TrafficTools.Menus
{
    public class ToolsMenuAtlasBuilder : IAtlasBuilder
    {
        public IEnumerable<string> Keys
        {
            get { yield return RoadEditorToolbarItemInfo.NAME; }
        }

        public UITextureAtlas Build()
        {
            var thumbnailAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            thumbnailAtlas.padding = 0;
            thumbnailAtlas.name = "ToolsMenu";

            var shader = Shader.Find("UI/Default UI Shader");
            if (shader != null) thumbnailAtlas.material = new Material(shader);

            const string PATH = @"Menus\Textures\ToolsMenus.png";

            const string BG = "ToolbarIconBase";
            var bgVersions = new[] { "Normal", "Focused", "Hovered", "Pressed", "Disabled" };

            const string FG = "ToolbarIconRoadEditor";
            var fgVersions = new[] { "", "Focused", "Hovered", "Pressed", "Disabled" };

            var texture = AssetManager.instance.GetTexture(PATH, TextureType.UI);
            texture.FixTransparency();

            thumbnailAtlas.material.mainTexture = texture;

            var x = 1;
            var y = 1;

            const int TEXTURE_W = 174;
            const int TEXTURE_H = 93;



            // Base -------------------------------------------------------------------------------
            const int BASE_ICON_W = 43;
            const int BASE_ICON_H = 49;

            foreach (var t in bgVersions)
            {
                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = string.Format(BG + "{0}", t),
                    region = new Rect(
                        (float)(x) / TEXTURE_W,
                        (float)(y) / TEXTURE_H,
                        (float)(BASE_ICON_W) / TEXTURE_W,
                        (float)(BASE_ICON_H) / TEXTURE_H),
                    texture = new Texture2D(BASE_ICON_W, BASE_ICON_H, TextureFormat.ARGB32, false)
                };

                thumbnailAtlas.AddSprite(sprite);

                //x += BASE_ICON_W;
            }
            y += BASE_ICON_H + 1;

            const int FG_ICON_W = 41;
            const int FG_ICON_H = 41;
            foreach (var t in fgVersions)
            {
                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = string.Format(FG + "{0}", t),
                    region = new Rect(
                        (float)(x) / TEXTURE_W,
                        (float)(y) / TEXTURE_H,
                        (float)(FG_ICON_W) / TEXTURE_W,
                        (float)(FG_ICON_H) / TEXTURE_H),
                    texture = new Texture2D(FG_ICON_W, FG_ICON_H, TextureFormat.ARGB32, false)
                };

                thumbnailAtlas.AddSprite(sprite);

                //x += BASE_ICON_W;
            }

            return thumbnailAtlas;
        }
    }
}
