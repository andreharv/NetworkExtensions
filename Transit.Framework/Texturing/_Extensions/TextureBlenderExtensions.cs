
namespace Transit.Framework.Texturing
{
    public static class ImageBlenderExtensions
    {
        public static ITextureBlender WithComponent(this ITextureBlender blender, ITextureBlenderComponent component)
        {
            blender.AddComponent(component);
            return blender;
        }

        public static ITextureBlender WithComponent(this ITextureBlender blender, string path, Point position = null, byte alphaLevel = 255)
        {
            blender.AddComponent(new TextureBlenderComponent(() => AssetManager
                .instance
                .GetTexture(path, TextureType.Default))
            {
                Position = position ?? new Point(0, 0),
                AlphaLevel = alphaLevel
            });
            return blender;
        }

        //public static ITextureBlender WithAlphaComponent(this ITextureBlender blender, Image alphaImage, Point position = null)
        //{
        //    blender.AddComponent(new TextureBlenderAlphaComponent(() => alphaImage)
        //    {
        //        Position = position ?? new Point(0, 0)
        //    });
        //    return blender;
        //}
    }
}
