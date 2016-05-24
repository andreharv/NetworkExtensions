using ICities;
using UnityEngine;

namespace Transit.Addon.TM.ThreadingExtensions
{
    public sealed class EnsureUIClosedThreadingExtension : ThreadingExtensionBase
    {
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            if (ToolModuleV2.Instance == null || ToolsModifierControl.toolController == null || ToolsModifierControl.toolController == null || ToolModuleV2.Instance.UI == null)
            {
                return;
            }

            if (ToolsModifierControl.toolController.CurrentTool != ToolModuleV2.Instance.TrafficManagerTool && ToolModuleV2.Instance.UI.IsVisible())
            {
                ToolModuleV2.Instance.UI.Close();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToolModuleV2.Instance.UI.Close();
            }
        }
    }
}
