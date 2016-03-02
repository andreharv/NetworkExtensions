using ColossalFramework.UI;
using System.Collections.Generic;
using Transit.Framework;
using UnityEngine;

namespace CSL_Traffic.UI
{
    class UIUtils
    {
        private static readonly string[] sm_thumbnailStates = { "Disabled", "", "Hovered", "Focused" };

        public static readonly Dictionary<string, SpriteTextureInfo> sm_thumbnailCoords = new Dictionary<string, SpriteTextureInfo>()
        {
            {"TabBackgrounds", new SpriteTextureInfo() {startX = 763, startY = 50, width = 60, height = 25}},
            {"Vehicle Restrictions", new SpriteTextureInfo() {startX = 763, startY = 0, width = 32, height = 22}},
            {"Speed Restrictions", new SpriteTextureInfo() {startX = 763, startY = 22, width = 32, height = 22}},
        };

        public struct SpriteTextureInfo
        {
            public int startX;
            public int startY;
            public int width;
            public int height;
        }

        private static UITextureAtlas sm_RoadCustomizerAtlas;

        public static UITextureAtlas GetRoadCustomizerAtlas()
        {
            if (sm_RoadCustomizerAtlas == null)
            {
                sm_RoadCustomizerAtlas = LoadRoadCustomizerAtlas();
            }

            return sm_RoadCustomizerAtlas;
        }

        private static UITextureAtlas LoadRoadCustomizerAtlas()
        {
            const string name = "UIThumbnails";

            Shader shader = Shader.Find("UI/Default UI Shader");
            if (shader == null)
            {
                Logger.LogInfo("Cannot find UI Shader. Using default thumbnails.");
                return null;
            }

            Texture2D atlasTexture = AssetManager.instance.GetTexture(@"Menus\RoadCustomizer\Textures\Thumbnails.png", TextureType.UI);
            FixTransparency(atlasTexture);

            Material atlasMaterial = new Material(shader);
            atlasMaterial.mainTexture = atlasTexture;

            var atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            atlas.name = "Traffic++ " + name;
            atlas.material = atlasMaterial;


            SetThumbnails("rct", new SpriteTextureInfo
                {
                    startX = 796,
                    startY = 0,
                    width = 36,
                    height = 36
                }, 
                atlas);
            SetThumbnails("rctBg", new SpriteTextureInfo
                {
                    startX = 835,
                    startY = 0,
                    width = 43,
                    height = 49
                }, 
                atlas, 
                new[] { "Hovered", "Pressed", "Focused", "" });

            SetThumbnails("TabBg", sm_thumbnailCoords["TabBackgrounds"], atlas, sm_thumbnailStates);

            return atlas;
        }

        public static bool SetThumbnails(string name, SpriteTextureInfo info, UITextureAtlas atlas, string[] states = null)
        {
            if (atlas == null || atlas.texture == null)
                return false;

            Texture2D atlasTex = atlas.texture;
            float atlasWidth = atlasTex.width;
            float atlasHeight = atlasTex.height;
            float rectWidth = info.width / atlasWidth;
            float rectHeight = info.height / atlasHeight;
            int x = info.startX;
            int y = info.startY;

            if (states == null)
                states = new string[] { "" };

            for (int i = 0; i < states.Length; i++, x += info.width)
            {
                if (x < 0 || x + info.width > atlasWidth || y < 0 || y > atlasHeight)
                    continue;

                Texture2D spriteTex = new Texture2D(info.width, info.height);
                spriteTex.SetPixels(atlasTex.GetPixels(x, y, info.width, info.height));

                UITextureAtlas.SpriteInfo sprite = new UITextureAtlas.SpriteInfo()
                {
                    name = name + states[i],
                    region = new Rect(x / atlasWidth, y / atlasHeight, rectWidth, rectHeight),
                    texture = spriteTex
                };
                atlas.AddSprite(sprite);
            }

            return true;
        }


        //=========================================================================
        // Methods created by petrucio -> http://answers.unity3d.com/questions/238922/png-transparency-has-white-borderhalo.html
        //
        // Copy the values of adjacent pixels to transparent pixels color info, to
        // remove the white border artifact when importing transparent .PNGs.
        public static void FixTransparency(Texture2D texture)
        {
            Color32[] pixels = texture.GetPixels32();
            int w = texture.width;
            int h = texture.height;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int idx = y * w + x;
                    Color32 pixel = pixels[idx];
                    if (pixel.a == 0)
                    {
                        bool done = false;
                        if (!done && x > 0) done = TryAdjacent(ref pixel, pixels[idx - 1]);        // Left   pixel
                        if (!done && x < w - 1) done = TryAdjacent(ref pixel, pixels[idx + 1]);        // Right  pixel
                        if (!done && y > 0) done = TryAdjacent(ref pixel, pixels[idx - w]);        // Top    pixel
                        if (!done && y < h - 1) done = TryAdjacent(ref pixel, pixels[idx + w]);        // Bottom pixel
                        pixels[idx] = pixel;
                    }
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();
        }

        private static bool TryAdjacent(ref Color32 pixel, Color32 adjacent)
        {
            if (adjacent.a == 0) return false;

            pixel.r = adjacent.r;
            pixel.g = adjacent.g;
            pixel.b = adjacent.b;
            return true;
        }
        //=========================================================================
    }
}
