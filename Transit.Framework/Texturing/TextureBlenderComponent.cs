using System.Drawing;
using System.Drawing.Imaging;

namespace Transit.Framework.Texturing
{
    public class TextureBlenderComponent : ITextureBlenderComponent
    {
        public Image Image { get; private set; }
        public Point Position { get; set; }
        public bool IsRelativeFromPrevious { get; set; }
        public bool IncreaseHOffset { get; set; }
        public bool IncreaseVOffset { get; set; }
        public byte AlphaLevel { get; set; }

        public TextureBlenderComponent(string imagePath)
            : this(Image.FromFile(imagePath))
        {
        }

        public TextureBlenderComponent(Image image)
        {
            Image = image;
            IsRelativeFromPrevious = true;
            IncreaseHOffset = true;
            IncreaseVOffset = false;
            AlphaLevel = 255;
        }

        public void Apply(ref Point offset, Bitmap canvas)
        {
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
                    Image,
                    new Rectangle(offset.X, offset.Y, Image.Width, Image.Height),
                    0, 0, Image.Width, Image.Height,
                    GraphicsUnit.Pixel,
                    attrib);
            }

            if (IncreaseHOffset)
            {
                offset = new Point(offset.X + Image.Width, offset.Y);
            }

            if (IncreaseVOffset)
            {
                offset = new Point(offset.X, offset.Y + Image.Height);
            }
        }
    }

    public class ImageBlenderAlphaComponent : ITextureBlenderComponent
    {
        public Bitmap Image { get; private set; }
        public Point Position { get; set; }
        public bool IsRelativeFromPrevious { get; set; }
        public bool IncreaseHOffset { get; set; }
        public bool IncreaseVOffset { get; set; }
        public Color DefaultColor { get; set; }

        public ImageBlenderAlphaComponent(string imagePath)
            : this(System.Drawing.Image.FromFile(imagePath))
        {
        }

        public ImageBlenderAlphaComponent(Image image)
        {
            if (image is Bitmap)
            {
                Image = image as Bitmap;
            }
            else
            {
                Image = new Bitmap(image);
            }
            
            IsRelativeFromPrevious = true;
            IncreaseHOffset = true;
            IncreaseVOffset = false;
            DefaultColor = Color.FromArgb(127, 127, 127);
        }

        public void Apply(ref Point offset, Bitmap canvas)
        {
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
                if (y >= Image.Height)
                {
                    continue;
                }

                for (int x = 0; x < canvas.Width; x++)
                {
                    if (x >= Image.Width)
                    {
                        continue;
                    }

                    var cvsPixel = canvas.GetPixel(x, y);
                    var imgPixel = Image.GetPixel(x, y);

                    var alphaLvl = imgPixel.G;
                    var aprLvl = imgPixel.B;

                    var finalPixel = Color.FromArgb(alphaLvl, ApplyAPR(cvsPixel, DefaultColor, aprLvl));

                    canvas.SetPixel(x, y, finalPixel);
                }
            }

            if (IncreaseHOffset)
            {
                offset = new Point(offset.X + Image.Width, offset.Y);
            }

            if (IncreaseVOffset)
            {
                offset = new Point(offset.X, offset.Y + Image.Height);
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
