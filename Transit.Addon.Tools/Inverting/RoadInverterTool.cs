using ColossalFramework;
using ICities;
using Transit.Framework;
using Transit.Framework.UI;
using UnityEngine;

namespace Transit.Addon.Tools.Inverting
{
    public class RoadInverterTool : ThreadingExtensionBase
    {
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
            
            if (!Input.GetMouseButtonUp((int)MouseKeyCode.RightButton))
            {
                return;
            }

            var hoveringSegmentId = ExtendedToolBase.RayCastSegment();
            if (hoveringSegmentId == null)
            {
                return;
            }

            // TODO
            // if HoveringSegment is NOT Invertable
            //  Return;

            var hoveringSegment = NetManager.instance.m_segments.m_buffer[hoveringSegmentId.Value];
            if ((hoveringSegment.m_flags & NetSegment.Flags.Created) == 0)
            {
                return;
            }

            var netInfo = hoveringSegment.Info;
            var startNode = hoveringSegment.m_startNode;
            var endNode = hoveringSegment.m_endNode;
            var startDirection = hoveringSegment.m_startDirection;
            var endDirection = hoveringSegment.m_endDirection;

            Singleton<NetManager>.instance.ReleaseSegment(hoveringSegmentId.Value, true);

            ushort segmentId;
            Singleton<NetManager>.instance.CreateSegment
               (out segmentId,
                ref Singleton<SimulationManager>.instance.m_randomizer,
                netInfo,
                endNode,
                startNode,
                endDirection,
                startDirection,
                Singleton<SimulationManager>.instance.m_currentBuildIndex,
                Singleton<SimulationManager>.instance.m_currentBuildIndex,
                false);
        }
    }
}
