using System.Linq;
using Transit.Addon.ToolsV2.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;

namespace Transit.Addon.ToolsV2
{
    public partial class ToolModuleV2 : ModuleBase
    {
        private void InstallTools()
        {
            var tBuilders = Parts
                .OfType<IToolBuilder>()
                .WhereActivated()
                .ToArray();

            foreach (var tBuilder in tBuilders)
            {
                var tool = ToolsModifierControl.toolController.AddTool(tBuilder.ToolType);
                tBuilder.OnToolInstalled(tool);
                tBuilder.IsInstalled = true;
            }
        }

        private void UninstallTools()
        {
            var tBuilders = Parts
                .OfType<IToolBuilder>()
                .WhereActivated()
                .ToArray();

            foreach (var tBuilder in tBuilders)
            {
                if (tBuilder.IsInstalled)
                {
                    var tool = ToolsModifierControl.toolController.GetTool(tBuilder.ToolType);
                    tBuilder.OnToolUninstalling(tool);
                    ToolsModifierControl.toolController.RemoveTool(tBuilder.ToolType);
                    tBuilder.IsInstalled = false;
                }
            }
        }
    }
}
