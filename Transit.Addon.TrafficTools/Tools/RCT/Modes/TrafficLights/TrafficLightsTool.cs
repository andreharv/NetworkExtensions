using UnityEngine;

namespace Transit.Addon.TrafficTools
{
    public class TrafficLightsTool : RoadCustomizerTool
    {
        protected override void OnSelectNode()
        {
            ushort nodeId;
            if (TrySelectNode(out nodeId))
            {
                NetNode node = NetManager.instance.m_nodes.m_buffer[nodeId];
                if ((node.m_flags & NetNode.Flags.TrafficLights) == 0)
                    return;

                _selectedNode = nodeId;
                _mode = Mode.SelectLane;
            }
        }

        protected override void OnRenderNode(RenderManager.CameraInfo camera)
        {
            if (_hoveredNode == 0)
                return;

            NetNode node = NetManager.instance.m_nodes.m_buffer[_hoveredNode];
            Color color = (node.m_flags & NetNode.Flags.TrafficLights) != 0 ? Color.green : Color.red;
            RenderNode(camera, _hoveredNode, color);
        }
    }
}
