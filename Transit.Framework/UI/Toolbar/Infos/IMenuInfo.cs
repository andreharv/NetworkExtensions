using System.Collections.Generic;

namespace Transit.Framework.UI.Toolbar.Infos
{
    public interface IMenuInfo
    {
        IEnumerable<IMenuCategoryInfo> CategoryInfos { get; }
    }
}
