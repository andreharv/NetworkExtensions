using ICities;
using Transit.Framework.Modularity;

namespace Transit.Addon.ToolsV3
{
    public partial class ToolModuleV3 : ModuleBase
    {
        public override void OnReleased()
        {
            base.OnReleased();

            // Not required
            //ToolsModifierControl.toolController.RemoveTool<LaneRoutingTool>();
            //ToolsModifierControl.toolController.RemoveTool<LaneRestrictorTool>();
            //ToolsModifierControl.toolController.RemoveTool<TrafficLightsTool>();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
            {
                InstallTools();
            }
        }

        public override void OnLevelUnloading()
        {
            UninstallTools();
        }
    }
}
