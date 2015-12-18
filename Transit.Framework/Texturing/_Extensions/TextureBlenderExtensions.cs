using System.Drawing;

namespace Transit.Framework.Texturing
{
    public static class ImageBlenderExtensions
    {
        public static ITextureBlender WithComponent(this ITextureBlender blender, ITextureBlenderComponent component)
        {
            blender.AddComponent(component);
            return blender;
        }

        public static ITextureBlender WithComponent(this ITextureBlender blender, string path, Point? position = null, byte alphaLevel = 255)
        {
            blender.AddComponent(new TextureBlenderComponent(() => AssetManager
                .instance
                .GetTextureData(path)
                .AsImage())
            {
                Position = position == null ? new Point(0, 0) : position.Value,
                AlphaLevel = alphaLevel
            });
            return blender;
        }

        public static ITextureBlender WithAlphaComponent(this ITextureBlender blender, Image alphaImage, Point? position = null)
        {
            blender.AddComponent(new ImageBlenderAlphaComponent(() => alphaImage)
            {
                Position = position == null ? new Point(0, 0) : position.Value
            });
            return blender;
        }
    }
}
