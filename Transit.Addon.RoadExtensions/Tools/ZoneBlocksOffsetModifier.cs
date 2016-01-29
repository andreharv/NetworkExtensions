using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using Transit.Framework.Extenders.AI;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Tools
{
    public class ZoneBlocksOffsetModifier : ThreadingExtensionBase
    {
        private static Color _originalColor;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                var tool = ToolsModifierControl.toolController.CurrentTool as NetTool;
                if (tool == null)
                {
                    return;
                }

                if (tool.m_mode != NetTool.Mode.Upgrade)
                {
                    return;
                }

                if (ZoneBlocksOffset.Mode != ZoneBlocksOffsetMode.HalfCell)
                {
                    ZoneBlocksOffset.Mode = ZoneBlocksOffsetMode.HalfCell;

                    // 0 181 255 255
                    _originalColor = ToolsModifierControl.toolController.m_validColor;

                    // change tool overlay color so user can see that the modifier is enabled
                    ToolsModifierControl.toolController.m_validColor = new Color(60f / 255f, 0f, 1f, 1f);
                }
            }

            if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                if (ZoneBlocksOffset.Mode != ZoneBlocksOffsetMode.Default)
                {
                    ZoneBlocksOffset.Mode = ZoneBlocksOffsetMode.Default;

                    ToolsModifierControl.toolController.m_validColor = _originalColor;
                }
            }
        }
    }
}
