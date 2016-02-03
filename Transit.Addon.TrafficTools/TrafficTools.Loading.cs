using ICities;
using Transit.Addon.TrafficTools.Menus;
using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework.Redirection;

namespace Transit.Addon.TrafficTools
{
    public partial class TrafficToolsModule
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (TrafficToolsOptions != ModOptions.None)
            {
                GameMainToolbarItemsManager.AddItem<RoadEditorToolbarItemInfo>();
                GameMainToolbarItemsManager.AddSmallSeparator(12);
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
