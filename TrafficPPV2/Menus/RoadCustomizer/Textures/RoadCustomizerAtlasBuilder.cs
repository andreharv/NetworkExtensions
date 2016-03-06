using ColossalFramework.UI;
using System.Collections.Generic;
using Transit.Framework;
using Transit.Framework.Builders;
using UnityEngine;

namespace CSL_Traffic.UI
{
    public class RoadCustomizerAtlasBuilder : IAtlasBuilder
    {
        private static readonly string[] sm_thumbnailStates = { "Disabled", "", "Hovered", "Focused" };
        private static readonly string[] sm_vehicleThumbnailStates = { "Disabled", "Deselected", "90%", "", "80%" };
        private static readonly string[] sm_emergencyVehicleThumbnailStates = { "Disabled", "Deselected", "90%", "", "80%", "Lights0", "Lights1" };

        private static readonly Dictionary<string, SpriteTextureInfo> sm_thumbnailCoords = new Dictionary<string, SpriteTextureInfo>()
        {
            {"Vehicle Restrictions", new SpriteTextureInfo {startX = 763, startY = 0, width = 32, height = 22}},
            {"Speed Restrictions", new SpriteTextureInfo {startX = 763, startY = 22, width = 32, height = 22}},
        };

        private static readonly Dictionary<string, SpriteTextureInfo> sm_bgthumbnailCoords = new Dictionary<string, SpriteTextureInfo>()
        {
            {"TabBg", new SpriteTextureInfo {startX = 763, startY = 50, width = 60, height = 25}},
            {"SpeedSignBackground", new SpriteTextureInfo {startX = 545, startY = 375, width = 109, height = 100}},
        };

        private static readonly Dictionary<string, SpriteTextureInfo> sm_vehiclesThumbnailCoords = new Dictionary<string, SpriteTextureInfo>()
        {
            {"Emergency", new SpriteTextureInfo {startX = 0, startY = 0, width = 109, height = 75}},
            {"Hearse", new SpriteTextureInfo {startX = 0, startY = 75, width = 109, height = 75}},
            {"GarbageTruck", new SpriteTextureInfo {startX = 0, startY = 150, width = 109, height = 75}},
            {"CargoTruck", new SpriteTextureInfo {startX = 0, startY = 225, width = 109, height = 75}},
            {"Bus", new SpriteTextureInfo {startX = 0, startY = 300, width = 109, height = 75}},
            {"PassengerCar", new SpriteTextureInfo {startX = 0, startY = 375, width = 109, height = 75}},
        };

        private static readonly Dictionary<string, SpriteTextureInfo> sm_speedThumbnailCoords = new Dictionary<string, SpriteTextureInfo>()
        {
            {"15", new SpriteTextureInfo {startX = 654, startY = 75, width = 109, height = 75}},
            {"30", new SpriteTextureInfo {startX = 872, startY = 75, width = 109, height = 75}},
            {"40", new SpriteTextureInfo {startX = 545, startY = 150, width = 109, height = 75}},
            {"50", new SpriteTextureInfo {startX = 654, startY = 150, width = 109, height = 75}},
            {"60", new SpriteTextureInfo {startX = 763, startY = 150, width = 109, height = 75}},
            {"70", new SpriteTextureInfo {startX = 872, startY = 150, width = 109, height = 75}},
            {"80", new SpriteTextureInfo {startX = 545, startY = 225, width = 109, height = 75}},
            {"90", new SpriteTextureInfo {startX = 654, startY = 225, width = 109, height = 75}},
            {"100", new SpriteTextureInfo {startX = 763, startY = 225, width = 109, height = 75}},
            {"120", new SpriteTextureInfo {startX = 545, startY = 300, width = 109, height = 75}},
            {"140", new SpriteTextureInfo {startX = 763, startY = 300, width = 109, height = 75}},
        };

        private struct SpriteTextureInfo
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

            var atlasTexture = AssetManager.instance.GetTexture(@"Menus\RoadCustomizer\Textures\Thumbnails.png", TextureType.UI);
            atlasTexture.FixTransparency();

            var atlasMaterial = new Material(shader);
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

            foreach (var kvp in sm_thumbnailCoords)
            {
                SetThumbnails(kvp.Key, kvp.Value, atlas);
            }

            foreach (var kvp in sm_bgthumbnailCoords)
            {
                SetThumbnails(kvp.Key, kvp.Value, atlas, sm_thumbnailStates);
            }

            foreach (var kvp in sm_vehiclesThumbnailCoords)
            {
                SetThumbnails(kvp.Key, kvp.Value, atlas, kvp.Key == "Emergency" ? sm_emergencyVehicleThumbnailStates : sm_vehicleThumbnailStates);
            }

            foreach (var kvp in sm_speedThumbnailCoords)
            {
                SetThumbnails(kvp.Key, kvp.Value, atlas);
            }

            return atlas;
        }

        private static void SetThumbnails(string name, SpriteTextureInfo info, UITextureAtlas atlas, string[] states = null)
        {
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
        }


        public IEnumerable<string> Keys
        {
            get
            {
                yield break;
            }
        }

        public UITextureAtlas Build()
        {
            return GetRoadCustomizerAtlas();
        }
    }
}
