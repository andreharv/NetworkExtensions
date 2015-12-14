using System.Drawing;
using System.Drawing.Imaging;

namespace Transit.Framework.Imaging
{
    public class ImageBlendingComponent
    {
        public string Path { get; set; }
        public Point Position { get; set; }
        public bool IsRelativeFromPrevious { get; set; }
        public bool IncreaseHOffset { get; set; }
        public bool IncreaseVOffset { get; set; }
        public byte AlphaLevel { get; set; }

        public ImageBlendingComponent()
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
