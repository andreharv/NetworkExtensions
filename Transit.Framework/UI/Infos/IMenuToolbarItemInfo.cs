using System.Collections.Generic;
using Transit.Framework.Interfaces;

namespace Transit.Framework.UI.Infos
{
    public interface IMenuToolbarItemInfo : IIdentifiable, IDisplayable, IOrderable, IDescriptor
    {
        string Tutorial { get; }
        IEnumerable<IMenuCategoryInfo> Categories { get; }
    }
}
