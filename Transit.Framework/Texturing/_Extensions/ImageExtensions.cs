using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Transit.Framework.Texturing
{
    public static class ImageExtensions
    {
        public static Image AsImage(this byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                return Image.FromStream(ms);
            }
        }

        public static byte[] ToByteArray(this Image image, ImageFormat format = null)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format ?? ImageFormat.Bmp);
                return ms.ToArray();
            }
        }
    }
}
