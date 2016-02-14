using ICities;
using Transit.Addon.ToolsV2.LaneRouting;
using Transit.Addon.ToolsV2.LaneRouting.Core;
using Transit.Addon.ToolsV2.Menus.RoadEditor;
using Transit.Addon.ToolsV2.Menus.RoadEditor.Textures;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework.Modularity;

namespace Transit.Addon.ToolsV2
{
    public partial class ToolModuleV2 : ModuleBase
    {
        public override void OnInstallingAssets()
        {
            base.OnInstallingAssets();

            AtlasManager.instance.Include<ToolsMenuAtlasBuilder>();
            AtlasManager.instance.Include<ToolsAtlasBuilder>();
        }

        public override void OnInstallingContent()
        {
            base.OnInstallingContent();

            if (TrafficToolsOptions != ModOptions.None)
            {
                GameMainToolbarItemsManager.AddItem<RoadEditorToolbarItemInfo>();
                GameMainToolbarItemsManager.AddBigSeparator(12);
            }

            //PathFindingManager.instance.DefineCustomLaneRouting(LanesManager.instance);
        }

        public override void OnReleased()
        {
            base.OnReleased();

            //PathFindingManager.instance.DisableCustomLaneRouting();

            // Not required
            //ToolsModifierControl.toolController.RemoveTool<LaneRoutingTool>();
            //ToolsModifierControl.toolController.RemoveTool<LaneRestrictorTool>();
            //ToolsModifierControl.toolController.RemoveTool<TrafficLightsTool>();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            ToolsModifierControl.toolController.AddTool<RoutingTool>();
        }

        public override void OnLevelUnloading()
        {
        }
    }
}
