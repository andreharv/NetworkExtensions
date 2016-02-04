using ColossalFramework.UI;
using System.Collections.Generic;

namespace Transit.Framework.Builders
{
    public interface IAtlasBuilder
    {
        IEnumerable<string> Keys { get; }
        UITextureAtlas Build();
    }
}
