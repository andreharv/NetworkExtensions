using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Transit.Framework.Texturing
{
    public class TextureBlender : ITextureBlender
    {
        public static ITextureBlender FromBaseFile(string baseTexturePath)
        {
            return new TextureBlender(() => AssetManager
                .instance
                .GetTextureData(baseTexturePath)
                .AsImage());
        }

        public static ITextureBlender FromImage(Image baseImage)
        {
            return new TextureBlender(() => baseImage);
        }

        private readonly Func<Image> _baseImageProvider;
        private readonly ICollection<ITextureBlenderComponent> _components = new List<ITextureBlenderComponent>();
        private Texture2D _texture;

        public TextureBlender(Func<Image> baseImageProvider)
        {
            _baseImageProvider = baseImageProvider;
            _components = new List<ITextureBlenderComponent>();
        }

        public void AddComponent(ITextureBlenderComponent component)
        {
            _components.Add(component);
            _texture = null;
        }

        public Image GetImage()
        {
            var image = _baseImageProvider();
            var canvas = new Bitmap(image);

            var componentOffset = new Point(0, 0);

            foreach (var c in _components)
            {
                c.Apply(ref componentOffset, canvas);
            }

            return canvas;
        }

        public Texture2D GetTexture()
        {
            if (_texture == null)
            {
                _texture = TextureCreator.FromData(GetImage().ToByteArray(), string.Empty, TextureType.Default);
            }

            return _texture;
        }
    }
}
