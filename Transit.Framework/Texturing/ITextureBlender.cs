using System.Collections.Generic;
using System.Drawing;

namespace Transit.Framework.Texturing
{
    public interface ITextureBlender : ITextureProvider
    {
        void AddComponent(ITextureBlenderComponent component);
        Image Build();
    }
}
