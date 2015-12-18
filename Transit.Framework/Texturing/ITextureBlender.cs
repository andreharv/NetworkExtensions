
namespace Transit.Framework.Texturing
{
    public interface ITextureBlender : ITextureProvider
    {
        void AddComponent(ITextureBlenderComponent component);
    }
}
