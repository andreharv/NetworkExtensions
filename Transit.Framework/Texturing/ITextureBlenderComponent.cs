using UnityEngine;

namespace Transit.Framework.Texturing
{
    public interface ITextureBlenderComponent
    {
        void Apply(ref Point offset, Texture2D canvas);
    }
}
