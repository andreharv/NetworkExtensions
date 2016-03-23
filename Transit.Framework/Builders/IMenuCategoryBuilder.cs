using System.Collections.Generic;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;

namespace Transit.Framework.Builders
{
    public interface IMenuCategoryBuilder : IIdentifiable, IDisplayable, IOrderable, IModulePart
    {
        GeneratedGroupPanel.GroupFilter? Group { get; }
        ItemClass.Service? Service { get; }
    }

    public interface IToolMenuCategoryBuilder : IMenuCategoryBuilder
    {
        IEnumerable<IToolBuilder> ToolBuilders { get; }
    }
}
