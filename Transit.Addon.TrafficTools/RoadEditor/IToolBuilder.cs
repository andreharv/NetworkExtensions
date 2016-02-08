using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;

namespace Transit.Addon.TrafficTools.RoadEditor
{
    public interface IToolBuilder : IMenuItemBuilder, IModulePart
    {
        int Order { get; }
    }
}
