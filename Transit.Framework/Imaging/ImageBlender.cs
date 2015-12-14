using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Transit.Framework.Imaging
{
    public class ImageBlender
    {
        private readonly string _baseImagePath;
        private readonly ICollection<BlendableComponent> _components = new List<BlendableComponent>();

        public ImageBlender(string baseImagePath)
        {
            _baseImagePath = baseImagePath;
        }

        public void Apply()
        {
            var baseImage = Image.FromFile(_baseImagePath);

            var width = baseImage.Width;
            var height = baseImage.Height;

            var componentOffset = new Point(0, 0);

            using (var canvas = new Bitmap(width, height))
            {
                using (var g = Graphics.FromImage(canvas))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(baseImage, new Rectangle(0, 0, width, height), new Rectangle(0, 0, baseImage.Width, baseImage.Height), GraphicsUnit.Pixel);

                    foreach (var c in _components)
                    {
                        c.Apply(ref componentOffset, g);
                    }
                }

                canvas.Save("BlenderResult.png", ImageFormat.Png);
            }
        }

        public void AddComponent(BlendableComponent component)
        {
            _components.Add(component);
        }
    }

    public class BlendableComponent
    {
        public string Path { get; set; }
        public Point Position { get; set; }
        public bool IsRelativeFromPrevious { get; set; }
        public bool IncreaseHOffset { get; set; }
        public bool IncreaseVOffset { get; set; }
        public byte AlphaLevel { get; set; }

        public BlendableComponent()
        {
            IsRelativeFromPrevious = true;
            IncreaseHOffset = true;
            IncreaseVOffset = false;
            AlphaLevel = 255;
        }

        public Image Load()
        {
            return Image.FromFile(Path);
        }

        public void Apply(ref Point offset, Graphics g)
        {
            if (!IsRelativeFromPrevious)
            {
                offset = Position;
            }
            else
            {
                offset = new Point(offset.X + Position.X, offset.Y + Position.Y);
            }

            var cImg = Load();

            var matrix = new ColorMatrix { Matrix33 = AlphaLevel / 255f };
            var attrib = new ImageAttributes();
            attrib.SetColorMatrix(matrix);

            g.DrawImage(
                cImg,
                new Rectangle(offset.X, offset.Y, cImg.Width, cImg.Height),
                0, 0, cImg.Width, cImg.Height,
                GraphicsUnit.Pixel,
                attrib);

            if (IncreaseHOffset)
            {
                offset = new Point(offset.X + cImg.Width, offset.Y);
            }

            if (IncreaseVOffset)
            {
                offset = new Point(offset.X, offset.Y + cImg.Height);
            }
        }
    }
}
