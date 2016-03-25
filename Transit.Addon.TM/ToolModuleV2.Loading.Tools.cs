using System;
using System.Linq;
using Transit.Addon.TM.Tools.LaneRestriction;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.UI;

namespace Transit.Addon.TM
{
    public partial class ToolModuleV2
    {
        public override void OnInstallingMenus()
        {
            base.OnInstallingMenus();

            var tBuilders = Parts
                .OfType<IToolBuilder>()
                //.WhereActivated()
                .ToArray();

            foreach (var tBuilder in tBuilders)
            {
                MenuManager.instance.RegisterTool(tBuilder);
            }
        }

        private void InstallTools()
        {
            var tBuilders = Parts
                .OfType<IToolBuilder>()
                //.WhereActivated()
                .ToArray();

            foreach (var tBuilder in tBuilders)
            {
                try
                {
                    var tool = ToolsModifierControl.toolController.AddTool(tBuilder.ToolType);
                    tBuilder.OnToolInstalled(tool);
                    tBuilder.IsInstalled = true;

                    Log.Info(string.Format("Tools: {0} installed", tBuilder.Name));
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("Tools: Crashed-Network builder {0}", tBuilder.Name));
                    Log.Error("Tools: " + ex.Message);
                    Log.Error("Tools: " + ex.ToString());
                }
            }

            // TODO: legacy to be removed
            ToolsModifierControl.toolController.AddTool<LaneRestrictionTool>();
        }

        private void UninstallTools()
        {
            var tBuilders = Parts
                .OfType<IToolBuilder>()
                //.WhereActivated()
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

                MenuManager.instance.UnregisterTool(tBuilder);
            }

            // TODO: legacy to be removed
            ToolsModifierControl.toolController.RemoveTool<LaneRestrictionTool>();
        }
    }
}
