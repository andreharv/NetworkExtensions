
using UnityEngine;

namespace Transit.Framework.Texturing
{
    public partial class TextureBlender
    {
        private class XOffsetComponent: ITextureBlenderComponent
        {
            private readonly int _xOffset;

            public XOffsetComponent(int xOffset)
            {
                _xOffset = xOffset;
            }

            public void Apply(ref Point offset, Texture2D canvas)
            {
                offset = new Point(offset.X + _xOffset, offset.Y);
            }
        }
    }
}
