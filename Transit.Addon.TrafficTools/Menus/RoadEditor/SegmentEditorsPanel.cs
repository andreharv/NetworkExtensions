using ColossalFramework.UI;
using Transit.Framework.UI;

namespace Transit.Addon.TrafficTools.Menus.RoadEditor
{
    public class SegmentEditorsPanel : CustomScrollPanelBase
    {
        public override void RefreshPanel()
        {
            base.RefreshPanel();

            //UIButton button = SpawnEntry("Manual", null, null, null, null, true);
            //button.text = "Manual";
            //button.objectUserData = LaneRoutingTool.Template.None;

            //button = SpawnEntry("Roundabout", null, null, null, null, true);
            //button.text = "Roundabout";
            //button.objectUserData = LaneRoutingTool.Template.Roundabout;
        }

        protected override void OnButtonClicked(UIComponent comp)
        {
            //LaneRoutingTool lrt = ToolsModifierControl.GetCurrentTool<LaneRoutingTool>();
            //if (lrt != null)
            //    lrt.TemplateMode = (LaneRoutingTool.Template)comp.objectUserData;
        }
    }
}
