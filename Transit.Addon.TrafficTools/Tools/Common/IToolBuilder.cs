using System;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;

namespace Transit.Addon.TrafficTools.Common
{
    public interface IToolBuilder : IMenuItemBuilder, IModulePart
    {
        int Order { get; }
        Type ToolType { get; }
    }
}
