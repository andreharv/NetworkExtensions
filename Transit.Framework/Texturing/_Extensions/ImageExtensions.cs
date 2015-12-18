using System.IO;

namespace Transit.Framework.Texturing
{
    public static class ImageExtensions
    {
        public static void Save(this byte[] imageBytes, string path)
        {
            File.WriteAllBytes(path, imageBytes);
        }
    }
}
