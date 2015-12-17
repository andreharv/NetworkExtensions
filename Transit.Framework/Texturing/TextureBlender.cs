using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Transit.Framework.Texturing
{
    public class TextureBlender : ITextureBlender
    {
        public static ITextureBlender FromBaseFile(string baseImagePath)
        {
            return new TextureBlender(baseImagePath);
        }

        public static ITextureBlender FromImage(Image baseImage)
        {
            return new TextureBlender(baseImage);
        }

        public static ITextureBlender FromByteImage(byte[] baseImageBytes)
        {
            return new TextureBlender(baseImageBytes);
        }

        private readonly Image _baseImage;
        private readonly ICollection<ITextureBlenderComponent> _components = new List<ITextureBlenderComponent>();
        private Texture2D _texture;

        public TextureBlender(string baseImagePath)
            : this(Image.FromFile(baseImagePath))
        {
        }

        public TextureBlender(Image baseImage)
        {
            _baseImage = baseImage;
            _components = new List<ITextureBlenderComponent>();
        }

        public TextureBlender(byte[] baseImageBytes)
        {
            _baseImage = baseImageBytes.AsImage();
            _components = new List<ITextureBlenderComponent>();
        }

        public void AddComponent(ITextureBlenderComponent component)
        {
            _components.Add(component);
            _texture = null;
        }

        public Image Build()
        {
            var componentOffset = new Point(0, 0);

            var canvas = new Bitmap(_baseImage);

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
                _texture = TextureCreator.FromData(Build().ToByteArray(), string.Empty, TextureType.Default);
            }

            return _texture;
        }
    }
}
