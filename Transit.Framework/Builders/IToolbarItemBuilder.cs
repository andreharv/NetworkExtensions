using System.Collections.Generic;
using Transit.Framework.Interfaces;

namespace Transit.Framework.Builders
{
    public interface IToolbarItemBuilder : IIdentifiable, IDisplayable, IOrderable, IDescriptor
    {
        string Tutorial { get; }
        IEnumerable<IMenuCategoryBuilder> CategoryBuilders { get; }
    }
}
