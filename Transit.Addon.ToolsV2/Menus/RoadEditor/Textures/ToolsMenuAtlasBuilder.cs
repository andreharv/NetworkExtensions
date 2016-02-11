using System.Collections.Generic;
using ColossalFramework.UI;
using Transit.Framework;
using Transit.Framework.Builders;
using UnityEngine;

namespace Transit.Addon.ToolsV2.Menus.RoadEditor.Textures
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

            const string PATH = @"Menus\RoadEditor\Textures\ToolsMenu.png";

            const string BG = "ToolbarIconBase";
            var bgVersions = new[] { "Normal", "Focused", "Hovered", "Pressed", "Disabled" };

            const string FG = "ToolbarIconRoadEditor";
            var fgVersions = new[] { "", "Focused", "Hovered", "Pressed", "Disabled" };

            var texture = AssetManager.instance.GetTexture(PATH, TextureType.UI);
            texture.FixTransparency();

            thumbnailAtlas.material.mainTexture = texture;

            var x = 1;
            var y = 1;

            const int TEXTURE_W = 178;
            const int TEXTURE_H = 86;



            // Base -------------------------------------------------------------------------------
            const int BG_ICON_W = 43;
            const int BG_ICON_H = 49;

            var imgBgIds = new[] {"Hovered", "Pressed", "Focused", ""};

            foreach (var t in bgVersions)
            {
                int id = imgBgIds.Length - 1;

                for (int i = 0; i < imgBgIds.Length; i++)
                {
                    if (imgBgIds[i] == t)
                    {
                        id = i;
                        break;
                    }
                }

                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = string.Format(BG + "{0}", t),
                    region = new Rect(
                        (float)(x + ((BG_ICON_W + 1) * id)) / TEXTURE_W,
                        (float)(y) / TEXTURE_H,
                        (float)(BG_ICON_W) / TEXTURE_W,
                        (float)(BG_ICON_H) / TEXTURE_H),
                    texture = new Texture2D(BG_ICON_W, BG_ICON_H, TextureFormat.ARGB32, false)
                };

                thumbnailAtlas.AddSprite(sprite);
            }
            y += BG_ICON_H + 1;

            const int FG_ICON_W = 36;
            const int FG_ICON_H = 36;
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
