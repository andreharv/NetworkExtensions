using ColossalFramework.UI;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Menus
{
    public static class AdditionnalMenus
    {
        public const string ROADS_SMALL_HV = "RoadsSmallHV";
        public const string ROADS_BUSWAYS = "RoadsBusways";

        private static UITextureAtlas s_thumbnailAtlas = null;

        public static UITextureAtlas LoadThumbnails()
        {
            if (s_thumbnailAtlas != null)
            {
                return s_thumbnailAtlas;
            }

            var thumbnailAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            thumbnailAtlas.padding = 0;
            thumbnailAtlas.name = "AdditionnalSubBar";

            var shader = Shader.Find("UI/Default UI Shader");
            if (shader != null) thumbnailAtlas.material = new Material(shader);

            const string PATH = @"Menus\Textures\AdditionnalSubBar.png";

            const string BASE = "SubBarButtonBase";
            const string ROADS_SMALL_HV_SUBBAR = "SubBar" + AdditionnalMenus.ROADS_SMALL_HV;

            var versions = new[] { "", "Disabled", "Focused", "Hovered", "Pressed" };


            var texture = AssetManager.instance.GetTexture(PATH);
            texture.FixTransparency();

            thumbnailAtlas.material.mainTexture = texture;

            var x = 1;
            var y = 1;

            const int TEXTURE_W = 292;
            const int TEXTURE_H = 50;



            // Base -------------------------------------------------------------------------------
            const int BASE_ICON_W = 58;
            const int BASE_ICON_H = 25;

            foreach (var t in versions)
            {
                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = string.Format(BASE + "{0}", t),
                    region = new Rect(
                        (float)(x) / TEXTURE_W,
                        (float)(y) / TEXTURE_H,
                        (float)(BASE_ICON_W) / TEXTURE_W,
                        (float)(BASE_ICON_H) / TEXTURE_H),
                    texture = new Texture2D(BASE_ICON_W, BASE_ICON_H, TextureFormat.ARGB32, false)
                };

                thumbnailAtlas.AddSprite(sprite);

                x += BASE_ICON_W;
            }

            x = 1;
            y += BASE_ICON_H + 1;



            // RoadsSmallHV -----------------------------------------------------------------------
            const int ICON_W = 32;
            const int ICON_H = 22;

            foreach (var t in versions)
            {
                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = string.Format(ROADS_SMALL_HV_SUBBAR + "{0}", t),
                    region = new Rect(
                        (float)(x) / TEXTURE_W,
                        (float)(y) / TEXTURE_H,
                        (float)(ICON_W) / TEXTURE_W,
                        (float)(ICON_H) / TEXTURE_H),
                    texture = new Texture2D(ICON_W, ICON_H, TextureFormat.ARGB32, false)
                };

                thumbnailAtlas.AddSprite(sprite);

                x += ICON_W;
            }

            s_thumbnailAtlas = thumbnailAtlas;

            return s_thumbnailAtlas;
        }
    }
}
