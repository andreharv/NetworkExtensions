using System.Collections.Generic;

namespace Transit.Framework.Builders
{
    public interface IMenuBuilder
    {
        IEnumerable<IMenuCategoryBuilder> CategoryBuilders { get; }
    }
}
