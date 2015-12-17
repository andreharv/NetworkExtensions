using System;
using UnityEngine;

namespace Transit.Framework.Texturing
{
    public static class TextureCreator
    {
        public static Texture2D FromData(byte[] textureBytes, string textureName, TextureType type)
        {
            switch (type)
            {
                case TextureType.Default:
                    {
                        var texture = new Texture2D(1, 1);
                        texture.name = textureName;
                        texture.LoadImage(textureBytes);
                        texture.anisoLevel = 8;
                        texture.filterMode = FilterMode.Trilinear;
                        texture.Apply();
                        return texture;
                    }

                case TextureType.LOD:
                    {
                        var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                        texture.name = textureName;
                        texture.LoadImage(textureBytes);
                        texture.anisoLevel = 8;
                        texture.filterMode = FilterMode.Trilinear;
                        texture.Apply();
                        texture.Compress(false);
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
