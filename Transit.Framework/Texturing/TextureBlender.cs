using System;
using System.Collections.Generic;
using ColossalFramework;
using UnityEngine;

namespace Transit.Framework.Texturing
{
    public partial class TextureBlender : ITextureProvider
    {
        public static TextureBlender FromBaseFile(string baseTexturePath, string textureName = null)
        {
            return new TextureBlender(() => AssetManager
                .instance
                .GetEditableTexture(baseTexturePath, textureName));
        }

        public static TextureBlender FromTexture(Texture2D baseTexture)
        {
            return new TextureBlender(() => baseTexture);
        }

        private readonly Func<Texture2D> _baseTextureProvider;
        private readonly ICollection<ITextureBlenderComponent> _components = new List<ITextureBlenderComponent>();
        private Texture2D _texture;

        public TextureBlender(Func<Texture2D> baseTextureProvider)
        {
            _baseTextureProvider = baseTextureProvider;
            _components = new List<ITextureBlenderComponent>();
        }

        private void AddComponent(ITextureBlenderComponent component)
        {
            _components.Add(component);
            _texture = null;
        }

        public Texture2D GetTexture()
        {
            if (_texture == null)
            {
                var canvas = _baseTextureProvider();
                var componentOffset = new Point(0, 0);
                foreach (var c in _components)
                {
                    c.Apply(ref componentOffset, canvas);
                }

                canvas.Apply();

#if DEBUG_TEXBLEND
                if (!canvas.name.IsNullOrWhiteSpace())
                {
                    canvas
                        .EncodeToPNG()
                        .Save(canvas.name + ".png");
                }
#endif

                _texture = canvas;
            }

            return _texture;
        }

        public TextureBlender WithXOffset(int xOffset)
        {
            AddComponent(new XOffsetComponent(xOffset));
            return this;
        }

        public TextureBlender WithComponent(string path, byte alphaLevel = 255, int positionX = 0, bool increaseXOffset = true)
        {
            AddComponent(new TextureComponent
                (() => AssetManager.instance.GetTexture(path, TextureType.Default),
                 alphaLevel: alphaLevel,
                 position: new Point(positionX, 0),
                 increaseXOffset: increaseXOffset));
            return this;
        }

        public TextureBlender WithAlphaComponent(Texture2D alphaTexture, Point position = null)
        {
            AddComponent(new AlphaComponent(
                () => alphaTexture,
                position: position));
            return this;
        }
    }
}
