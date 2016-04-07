using ColossalFramework;

namespace Transit.Addon.TM.Tools.LaneDirectionEditor
{
    public partial class LaneDirectionEditor
    {
        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            //Log._Debug($"LaneArrow Overlay: {HoveredNodeId} {HoveredSegmentId} {SelectedNodeId} {SelectedSegmentId}");
            if (_hoveredSegmentId != 0 && 
                _hoveredNodeId != 0 && 
                (_hoveredSegmentId != _selectedSegmentId || _hoveredNodeId != _selectedNodeId))
            {
                var netFlags = Singleton<NetManager>.instance.m_nodes.m_buffer[_hoveredNodeId].m_flags;

                if ((netFlags & NetNode.Flags.Junction) != NetNode.Flags.None)
                {
                    NetTool.RenderOverlay(cameraInfo, ref Singleton<NetManager>.instance.m_segments.m_buffer[_hoveredSegmentId], GetToolColor(false, false), GetToolColor(false, false));
                }
            }

            if (_selectedSegmentId == 0) return;

            NetTool.RenderOverlay(cameraInfo, ref Singleton<NetManager>.instance.m_segments.m_buffer[_selectedSegmentId], GetToolColor(true, false), GetToolColor(true, false));
        }
    }
}
