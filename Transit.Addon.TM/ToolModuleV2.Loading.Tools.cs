using System;
using System.Linq;
using Transit.Addon.TM.Tools.RoadCustomizer;
using Transit.Addon.TM.UI.Toolbar.RoadEditor;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework.ExtensionPoints.UI.Toolbar;
using Transit.Framework.UI;
using UnityEngine;

namespace Transit.Addon.TM
{
    public partial class ToolModuleV2
    {
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
            ToolsModifierControl.toolController.AddTool<RoadCustomizerTool>();
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
            }

            // TODO: legacy to be removed
            ToolsModifierControl.toolController.RemoveTool<RoadCustomizerTool>();
        }
    }
}
