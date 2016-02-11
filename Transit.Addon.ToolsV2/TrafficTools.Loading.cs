using ICities;
using Transit.Addon.ToolsV2.Menus.RoadEditor;
using Transit.Addon.ToolsV2.Menus.RoadEditor.Textures;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework.Modularity;
using Transit.Framework.Redirection;

namespace Transit.Addon.ToolsV2
{
    public partial class TrafficToolsModule : ModuleBase
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
        }

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
            Redirector.PerformRedirections();
        }

        public override void OnLevelUnloading()
        {
            Redirector.RevertRedirections();
        }
    }
}
