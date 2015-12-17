using System.Collections.Generic;
using System.Drawing;

namespace Transit.Framework.Imaging
{
    public class ImageBlender : IImageBlender
    {
        public static IImageBlender FromBaseFile(string baseImagePath)
        {
            return new ImageBlender(baseImagePath);
        }

        public static IImageBlender FromImage(Image baseImage)
        {
            return new ImageBlender(baseImage);
        }

        public Image BaseImage { get; private set; }

        public ICollection<IImageBlenderComponent> Components { get; private set; }

        public ImageBlender(string baseImagePath)
            : this(Image.FromFile(baseImagePath))
        {
        }

        public ImageBlender(Image baseImage)
        {
            BaseImage = baseImage;
            Components = new List<IImageBlenderComponent>();
        }

        public Image Build()
        {
            var componentOffset = new Point(0, 0);

            var canvas = new Bitmap(BaseImage);

            foreach (var c in Components)
            {
                c.Apply(ref componentOffset, canvas);
            }

            return canvas;
        }
    }
}
