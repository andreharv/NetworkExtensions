using System.Drawing;

namespace Transit.Framework.Imaging
{
    public interface IImageBlenderComponent
    {
        void Apply(ref Point offset, Bitmap canvas);
    }
}
