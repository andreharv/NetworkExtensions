using System;
using System.IO;
using UnityEngine;

namespace Transit.Framework.Texturing
{
    public static class TextureHelper
    {
        public static float Lerp(float baseColorPart, float defaultColorPart, float level)
        {
            return baseColorPart + (defaultColorPart - baseColorPart) * level;
        }

        public static void Save(this byte[] imageBytes, string path)
        {
            File.WriteAllBytes(path, imageBytes);
        }

        public static Texture2D AsEditableTexture(this byte[] textureBytes, string textureName = null)
        {
            var texture = new Texture2D(1, 1);
            if (textureName != null)
            {
                texture.name = textureName; 
            }
            texture.LoadImage(textureBytes);
            texture.anisoLevel = 8;
            texture.filterMode = FilterMode.Trilinear;
            return texture;
        }

        public static Texture2D AsTexture(this byte[] textureBytes, string textureName, TextureType type)
        {
            switch (type)
            {
                case TextureType.Default:
                    {
                        var texture = new Texture2D(1, 1, TextureFormat.ARGB32, true);
                        texture.name = textureName;
                        texture.LoadImage(textureBytes);
                        texture.anisoLevel = 8;
                        texture.filterMode = FilterMode.Bilinear;
                        texture.Compress(true);
                        texture.Apply(true, true);
                        return texture;
                    }

                case TextureType.LOD:
                    {
                        var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                        texture.name = textureName;
                        texture.LoadImage(textureBytes);
                        texture.anisoLevel = 8;
                        texture.filterMode = FilterMode.Bilinear;
                        texture.Compress(true);
                        texture.Apply();
                        return texture;
                    }

                case TextureType.UI:
                    {
                        var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                        texture.name = textureName;
                        texture.LoadImage(textureBytes);
                        texture.Apply();
                        return texture;
                    }

                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}
