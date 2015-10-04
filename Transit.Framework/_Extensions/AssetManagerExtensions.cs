using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework.UI;
using UnityEngine;

namespace Transit.Framework
{
    public static class AssetManagerExtensions
    {
        public static UITextureAtlas GetThumbnails(this AssetManager assetManager, string thumbnailsName, string thumbnailsPath)
        {
            var thumbnailAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            thumbnailAtlas.padding = 0;
            thumbnailAtlas.name = thumbnailsName;

            var shader = Shader.Find("UI/Default UI Shader");
            if (shader != null) thumbnailAtlas.material = new Material(shader);



            var texture = assetManager.GetTexture(thumbnailsPath);
            texture.FixTransparency();

            thumbnailAtlas.material.mainTexture = texture;

            const int iconW = 109;
            const int iconH = 100;

            const int textureW = iconW * 5;
            const int textureH = 100;


            string[] ts = { "", "Disabled", "Focused", "Hovered", "Pressed" };
            for (int x = 0; x < ts.Length; ++x)
            {
                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = string.Format(thumbnailsName.ToUpper() + "{0}", ts[x]),
                    region = new Rect(
                        (float)(x * iconW) / textureW, 0f,
                        (float)(iconW) / textureW, (float)(iconH) / textureH),
                    texture = new Texture2D(iconW, iconH, TextureFormat.ARGB32, false)
                };

                thumbnailAtlas.AddSprite(sprite);
            }

            return thumbnailAtlas;
        }

        public static UITextureAtlas GetInfoTooltip(this AssetManager assetManager, string infoTooltipName, string infoTooltipPath)
        {
            var infoTooltipAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            infoTooltipAtlas.padding = 0;
            infoTooltipAtlas.name = infoTooltipName;

            var shader = Shader.Find("UI/Default UI Shader");
            if (shader != null) infoTooltipAtlas.material = new Material(shader);

            var texture = assetManager.GetTexture(infoTooltipPath);

            infoTooltipAtlas.material.mainTexture = texture;

            const int ittW = 535;
            const int ittH = 150;

            var sprite = new UITextureAtlas.SpriteInfo
            {
                name = string.Format(infoTooltipName.ToUpper()),
                region = new Rect(0f, 0f, 1f, 1f),
                texture = new Texture2D(ittW, ittH, TextureFormat.ARGB32, false)
            };

            infoTooltipAtlas.AddSprite(sprite);

            return infoTooltipAtlas;
        }
    }
}
