using System.Drawing;

namespace Transit.Framework.Texturing
{
    public interface ITextureBlenderComponent
    {
        void Apply(ref Point offset, Bitmap canvas);
    }
}
