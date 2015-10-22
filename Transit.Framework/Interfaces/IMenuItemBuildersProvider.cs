using System.Collections.Generic;

namespace Transit.Framework.Interfaces
{
    public interface IMenuItemBuildersProvider
    {
        IEnumerable<IMenuItemBuilder> MenuItemBuilders { get; }
    }
}
