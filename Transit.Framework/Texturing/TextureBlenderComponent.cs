using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Transit.Framework.Texturing
{
    public class TextureBlenderComponent : ITextureBlenderComponent
    {
        private readonly Func<Image> _imageProvider;
        public Point Position { get; set; }
        public bool IsRelativeFromPrevious { get; set; }
        public bool IncreaseHOffset { get; set; }
        public bool IncreaseVOffset { get; set; }
        public byte AlphaLevel { get; set; }

        public TextureBlenderComponent(Func<Image> imageProvider)
        {
            _imageProvider = imageProvider;
            IsRelativeFromPrevious = true;
            IncreaseHOffset = true;
            IncreaseVOffset = false;
            AlphaLevel = 255;
        }

        public void Apply(ref Point offset, Bitmap canvas)
        {
            var image = _imageProvider();

            if (!IsRelativeFromPrevious)
            {
                offset = Position;
            }
            else
            {
                offset = new Point(offset.X + Position.X, offset.Y + Position.Y);
            }

            var matrix = new ColorMatrix { Matrix33 = AlphaLevel / 255f };
            var attrib = new ImageAttributes();
            attrib.SetColorMatrix(matrix);

            using (var g = Graphics.FromImage(canvas))
            {
                g.DrawImage(
                    image,
                    new Rectangle(offset.X, offset.Y, image.Width, image.Height),
                    0, 0, image.Width, image.Height,
                    GraphicsUnit.Pixel,
                    attrib);
            }

            if (IncreaseHOffset)
            {
                offset = new Point(offset.X + image.Width, offset.Y);
            }

            if (IncreaseVOffset)
            {
                offset = new Point(offset.X, offset.Y + image.Height);
            }
        }
    }

    public class ImageBlenderAlphaComponent : ITextureBlenderComponent
    {
        private readonly Func<Image> _imageProvider;
        public Point Position { get; set; }
        public bool IsRelativeFromPrevious { get; set; }
        public bool IncreaseHOffset { get; set; }
        public bool IncreaseVOffset { get; set; }
        public Color DefaultColor { get; set; }

        public ImageBlenderAlphaComponent(Func<Image> imageProvider)
        {
            _imageProvider = imageProvider;
            IsRelativeFromPrevious = true;
            IncreaseHOffset = true;
            IncreaseVOffset = false;
            DefaultColor = Color.FromArgb(127, 127, 127);
        }

        public void Apply(ref Point offset, Bitmap canvas)
        {
            var image = _imageProvider();
            Bitmap bitmap;

            if (image is Bitmap)
            {
                bitmap = image as Bitmap;
            }
            else
            {
                bitmap = new Bitmap(image);
            }

            if (!IsRelativeFromPrevious)
            {
                offset = Position;
            }
            else
            {
                offset = new Point(offset.X + Position.X, offset.Y + Position.Y);
            }

            for (int y = 0; y < canvas.Height; y++)
            {
                if (y >= bitmap.Height)
                {
                    continue;
                }

                for (int x = 0; x < canvas.Width; x++)
                {
                    if (x >= bitmap.Width)
                    {
                        continue;
                    }

                    var cvsPixel = canvas.GetPixel(x, y);
                    var imgPixel = bitmap.GetPixel(x, y);

                    var alphaLvl = imgPixel.G;
                    var aprLvl = imgPixel.B;

                    var finalPixel = Color.FromArgb(alphaLvl, ApplyAPR(cvsPixel, DefaultColor, aprLvl));

                    canvas.SetPixel(x, y, finalPixel);
                }
            }

            if (IncreaseHOffset)
            {
                offset = new Point(offset.X + bitmap.Width, offset.Y);
            }

            if (IncreaseVOffset)
            {
                offset = new Point(offset.X, offset.Y + bitmap.Height);
            }
        }

        private static Color ApplyAPR(Color baseColor, Color defaultColor, byte aprLevel)
        {
            return Color.FromArgb(
                ApplyAPRPart(baseColor.A, defaultColor.A, aprLevel),
                ApplyAPRPart(baseColor.R, defaultColor.R, aprLevel),
                ApplyAPRPart(baseColor.G, defaultColor.G, aprLevel),
                ApplyAPRPart(baseColor.B, defaultColor.B, aprLevel));
        }

        private static byte ApplyAPRPart(byte baseColorPart, byte defaultColorPart, byte aprLevel)
        {
            return (byte)(baseColorPart + (defaultColorPart - baseColorPart) * aprLevel / 255);
        }
    }
}
