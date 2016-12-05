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
            if (ToolsModifierControl.toolController == null)
            {
                return;
            }

            if (!GetIsUpgradeNetToolActive())
            {
                return;
            }
            var hoveringSegmentId = ExtendedToolBase.RayCastSegment();
            if (hoveringSegmentId == null)
            {
                return;
            }
            if (Input.GetMouseButton((int)MouseKeyCode.RightButton))
            {
                if (m_lastSegment.Contains(hoveringSegmentId.Value))
                {
                    return;
                }
            }
            else
            {
                m_lastSegment.Clear();
                return;
            }

            var mgr = NetManager.instance;
            var hoveringSegment = mgr.m_segments.m_buffer[hoveringSegmentId.Value];
            var netInfo = hoveringSegment.Info;
            if (!netInfo.HasAsymmetricalLanes())
            {
                return;
            }
            
            if ((hoveringSegment.m_flags & NetSegment.Flags.Created) == 0)
            {
                return;
            }


            var startNode = hoveringSegment.m_startNode;
            var endNode = hoveringSegment.m_endNode;
            var startDirection = hoveringSegment.m_startDirection;
            var endDirection = hoveringSegment.m_endDirection;

            var id = hoveringSegmentId.Value;
            ushort segmentId;
            var buildIndex = hoveringSegment.m_buildIndex;
            mgr.ReleaseSegment(hoveringSegmentId.Value, true);
            mgr.CreateSegment
               (out segmentId,
                ref Singleton<SimulationManager>.instance.m_randomizer,
                netInfo,
                endNode,
                startNode,
                endDirection,
                startDirection,
                buildIndex,
                Singleton<SimulationManager>.instance.m_currentBuildIndex,
                false);
            Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
            m_lastSegment.Add(segmentId);
        }
    }
}
