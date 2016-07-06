using ICities;

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

            // if NOT Mouse RightClick 
            //  Return;

            // if Mouse DONT HoverSegment
            //  Return;

            // if HoveringSegment is NOT Invertable
            //  Return;

            // if HoveringSegment.m_flags & Invert = None
            //  HoveringSegment.m_flags |= Invert
            // else
            //  HoveringSegment.m_flags &= ~Invert
        }
    }
}
