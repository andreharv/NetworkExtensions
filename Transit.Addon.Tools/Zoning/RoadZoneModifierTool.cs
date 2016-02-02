using ColossalFramework;
using ICities;
using Transit.Framework.ExtensionPoints.AI;
using UnityEngine;

namespace Transit.Addon.Tools.Zoning
{
    public class RoadZoneModifierTool : ThreadingExtensionBase
    {
        private static Color _originalColor;

        private bool _isActive = false;

        private static bool GetIsUpgradeNetToolActive()
        {
            var tool = ToolsModifierControl.toolController.CurrentTool as NetTool;
            if (tool == null)
            {
                return false;
            }

            if (tool.m_mode != NetTool.Mode.Upgrade)
            {
                return false;
            }

            return true;
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (!ToolModule.ActiveOptions.IsFlagSet(ToolModule.ModOptions.RoadZoneModifier))
            {
                return;
            }

            if (ToolsModifierControl.toolController == null)
            {
                return;
            }

            var isUpgradeNetToolActive = GetIsUpgradeNetToolActive();

            // Keys Down --------------------------------------------------------------------------
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                if (isUpgradeNetToolActive && !_isActive)
                {
                    _isActive = true;
                    ZoneBlocksOffset.Mode = ZoneBlocksOffsetMode.HalfCell;

                    // 0 181 255 255
                    _originalColor = ToolsModifierControl.toolController.m_validColor;

                    // change tool overlay color so user can see that the modifier is enabled
                    ToolsModifierControl.toolController.m_validColor = new Color(60f / 255f, 0f, 1f, 1f);
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                if (isUpgradeNetToolActive && _isActive)
                {
                    ZoneBlocksOffset.Mode = ZoneBlocksOffsetMode.ForcedDefault;
                }
            }


            // Keys Up ----------------------------------------------------------------------------
            if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                if (_isActive)
                {
                    _isActive = false;

                    ZoneBlocksOffset.Mode = ZoneBlocksOffsetMode.Default;

                    ToolsModifierControl.toolController.m_validColor = _originalColor;
                }
            }

            if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
            {
                if (isUpgradeNetToolActive && _isActive)
                {
                    ZoneBlocksOffset.Mode = ZoneBlocksOffsetMode.HalfCell;
                }
            }
        }
    }
}
