using System;
using System.Collections.Generic;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;

namespace Transit.Framework.UI.Toolbar.Infos
{
    public interface IMenuCategoryInfo : IIdentifiable, IOrderable
    {
        IEnumerable<IToolBuilder> ToolBuilders { get; }
    }
}
