using System.Collections.Generic;
using System.Drawing;

namespace Transit.Framework.Imaging
{
    public interface IImageBlender
    {
        ICollection<IImageBlenderComponent> Components { get; }
        Image Build();
    }
}
