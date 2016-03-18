using Transit.Addon.TM.Tools;
using Transit.Addon.TM.UI;
using Object = UnityEngine.Object;

namespace Transit.Addon.TM
{
    public partial class ToolModuleV2
    {
        public Mode ToolMode { get; set; }
        public TrafficManagerTool TrafficManagerTool { get; set; }

        public void SetToolMode(Mode mode)
        {
            if (mode == ToolMode) return;

            //UI.toolMode = mode;
            ToolMode = mode;

            if (mode != Mode.Disabled)
            {
                DestroyTool();
                EnableTool();
            }
            else {
                DestroyTool();
            }
        }

        public void EnableTool()
        {
            if (TrafficManagerTool == null)
            {
                TrafficManagerTool = ToolsModifierControl.toolController.gameObject.GetComponent<TrafficManagerTool>() ??
                                   ToolsModifierControl.toolController.gameObject.AddComponent<TrafficManagerTool>();
            }

            ToolsModifierControl.toolController.CurrentTool = TrafficManagerTool;
            ToolsModifierControl.SetTool<TrafficManagerTool>();
        }

        private void DestroyTool()
        {
            if (TrafficManagerTool != null)
            {
                ToolsModifierControl.toolController.CurrentTool = ToolsModifierControl.GetTool<DefaultTool>();
                ToolsModifierControl.SetTool<DefaultTool>();

                Object.Destroy(TrafficManagerTool);
                TrafficManagerTool = null;
            }
        }
    }
}
