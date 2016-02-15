using System;
using Transit.Framework.Modularity;

namespace Transit.Framework.Builders
{
    public interface IToolBuilder : IMenuItemBuilder, IModulePart
    {
        int Order { get; }
        Type ToolType { get; }
        bool IsInstalled { get; set; }

        void OnToolInstalled(ToolBase tool);
        void OnToolUninstalling(ToolBase tool);
    }
}
