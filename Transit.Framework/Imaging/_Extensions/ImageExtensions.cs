using System.Drawing;
using System.IO;

namespace Transit.Framework.Imaging
{
    public static class ImageExtensions
    {
        public static Image AsImage(this byte[] imageBytes)
        {
            var ms = new MemoryStream(imageBytes);
            return Image.FromStream(ms);
        }
    }
}
