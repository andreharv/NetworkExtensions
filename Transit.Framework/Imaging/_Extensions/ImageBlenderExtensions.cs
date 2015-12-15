using System.Drawing;

namespace Transit.Framework.Imaging
{
    public static class ImageBlenderExtensions
    {
        public static IImageBlender WithComponent(this IImageBlender blender, IImageBlenderComponent component)
        {
            blender.Components.Add(component);
            return blender;
        }

        public static IImageBlender WithComponent(this IImageBlender blender, string path, Point? position = null, byte alphaLevel = 255)
        {
            blender.Components.Add(new ImageBlenderComponent(path)
            {
                Position = position == null ? new Point(0, 0) : position.Value,
                AlphaLevel = alphaLevel
            });
            return blender;
        }

        public static IImageBlender WithAlphaComponent(this IImageBlender blender, Image alphaImage, Point? position = null)
        {
            blender.Components.Add(new ImageBlenderAlphaComponent(alphaImage)
            {
                Position = position == null ? new Point(0, 0) : position.Value
            });
            return blender;
        }
    }
}
