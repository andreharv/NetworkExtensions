using System;
using System.Collections.Generic;
using ColossalFramework;
using UnityEngine;

namespace Transit.Framework.Texturing
{
    public class TextureBlender : ITextureBlender
    {
        public static ITextureBlender FromBaseFile(string baseTexturePath, string textureName = null)
        {
            return new TextureBlender(() => AssetManager
                .instance
                .GetEditableTexture(baseTexturePath, textureName));
        }

        public static ITextureBlender FromTexture(Texture2D baseTexture)
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

        public void AddComponent(ITextureBlenderComponent component)
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

#if DEBUG_TEXGEN
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
    }
}
