using System.Collections.Generic;

namespace Transit.Framework.Builders
{
    public interface IMenuItemBuildersProvider
    {
        IEnumerable<IMenuItemBuilder> MenuItemBuilders { get; }
    }
}
