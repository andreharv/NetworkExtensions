using System;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;

namespace Transit.Framework.Builders
{
    public interface IToolBuilder : IMenuItemBuilder, IActivablePart, IOrderable
    {
        Type ToolType { get; }
        bool IsInstalled { get; set; }

        void OnToolInstalled(ToolBase tool);
        void OnToolUninstalling(ToolBase tool);
    }
}
