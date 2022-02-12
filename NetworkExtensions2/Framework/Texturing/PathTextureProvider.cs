using UnityEngine;

namespace Transit.Framework.Texturing
{
    public class PathTextureProvider : ITextureProvider
    {
        private readonly string _texturePath;
        private readonly TextureType _textureType;
        private Texture2D _texture;

        public PathTextureProvider(string texturePath, TextureType textureType)
        {
            _texturePath = texturePath;
            _textureType = textureType;
        }

        public Texture2D GetTexture()
        {
            if (_texture == null)
            {
                _texture = AssetManager.instance.GetTexture(_texturePath, _textureType);
            }

            return _texture;
        }
    }
}
