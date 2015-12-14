using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Transit.Framework.Imaging
{
    public class ImageBlending
    {
        private readonly string _baseImagePath;
        private readonly ICollection<ImageBlendingComponent> _components = new List<ImageBlendingComponent>();

        private ImageBlending(string baseImagePath)
        {
            _baseImagePath = baseImagePath;
        }

        public static ImageBlending FromBaseFile(string baseImagePath)
        {
            return new ImageBlending(baseImagePath);
        }

        public ImageBlending WithComponent(ImageBlendingComponent component)
        {
            _components.Add(component);
            return this;
        }

        public ImageBlending WithComponent(string path, Point? position = null, byte alphaLevel = 255)
        {
            _components.Add(new ImageBlendingComponent
            {
                Path = path,
                Position = position == null ? new Point(0, 0) : position.Value,
                AlphaLevel = alphaLevel
            });
            return this;
        }

        public Image Apply()
        {
            var baseImage = Image.FromFile(_baseImagePath);

            var width = baseImage.Width;
            var height = baseImage.Height;

            var componentOffset = new Point(0, 0);

            var canvas = new Bitmap(width, height);

            using (var g = Graphics.FromImage(canvas))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(baseImage, new Rectangle(0, 0, width, height), new Rectangle(0, 0, baseImage.Width, baseImage.Height), GraphicsUnit.Pixel);

                foreach (var c in _components)
                {
                    c.Apply(ref componentOffset, g);
                }
            }

            return canvas;
        }
    }
}
