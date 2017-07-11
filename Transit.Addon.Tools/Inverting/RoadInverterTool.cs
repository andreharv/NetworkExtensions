// Generating a bug where it creates an hole in the terrain, to investigate

using ColossalFramework;
using ICities;
using System.Collections.Generic;
using Transit.Framework;
using Transit.Framework.UI;
using UnityEngine;

namespace Transit.Addon.Tools.Inverting
{
    public class RoadInverterTool : ThreadingExtensionBase
    {
        private List<uint> m_lastSegment = new List<uint>();
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
        }
    }
}
