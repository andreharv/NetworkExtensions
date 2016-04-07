using ColossalFramework.Math;
using UnityEngine;

namespace Transit.Addon.TM.Tools.LaneRoutingEditor
{
    public partial class LaneRoutingEditor
    {
        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            if (m_selectedNode != 0)
            {
                FastList<NodeLaneMarker> nodeMarkers;
                if (m_nodeMarkers.TryGetValue(m_selectedNode, out nodeMarkers))
                {
                    Vector3 nodePos = NetManager.instance.m_nodes.m_buffer[m_selectedNode].m_position;
                    for (int i = 0; i < nodeMarkers.m_size; i++)
                    {
                        NodeLaneMarker laneMarker = nodeMarkers.m_buffer[i];

                        for (int j = 0; j < laneMarker.Connections.m_size; j++)
                            RenderRoute(cameraInfo, laneMarker.Position, laneMarker.Connections.m_buffer[j].Position, nodePos, laneMarker.Color);

                        if (m_selectedMarker != laneMarker && !IsActive(laneMarker))
                            continue;

                        if (m_selectedMarker == laneMarker)
                        {
                            RaycastOutput output;
                            if (RayCastSegmentAndNode(out output))
                            {
                                RenderRoute(cameraInfo, m_selectedMarker.Position, output.m_hitPos, nodePos, m_selectedMarker.Color);
                                m_selectedMarker.Size = 2f;
                            }
                        }

                        RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, laneMarker.Color, laneMarker.Position, laneMarker.Size, laneMarker.Position.y - 1f, laneMarker.Position.y + 1f, true, true);
                    }
                }
            }

            foreach (ushort node in m_nodeMarkers.Keys)
            {
                if (node == m_selectedNode || (NetManager.instance.m_nodes.m_buffer[node].m_flags & CUSTOMIZED_NODE_FLAG) != CUSTOMIZED_NODE_FLAG)
                    continue;

                FastList<NodeLaneMarker> list = m_nodeMarkers[node];
                Vector3 nodePos = NetManager.instance.m_nodes.m_buffer[node].m_position;
                for (int i = 0; i < list.m_size; i++)
                {
                    NodeLaneMarker laneMarker = list.m_buffer[i];
                    Color color = laneMarker.Color;
                    color.a = 0.75f;

                    for (int j = 0; j < laneMarker.Connections.m_size; j++)
                    {
                        if (((NetLane.Flags)NetManager.instance.m_lanes.m_buffer[laneMarker.Connections.m_buffer[j].LaneId].m_flags & NetLane.Flags.Created) == NetLane.Flags.Created)
                            RenderRoute(cameraInfo, laneMarker.Position, laneMarker.Connections.m_buffer[j].Position, nodePos, color);
                    }

                }
            }

            if (m_hoveredNode != 0)
            {
                NetNode node = NetManager.instance.m_nodes.m_buffer[m_hoveredNode];
                RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, new Color(0f, 0f, 0.5f, 0.75f), node.m_position, 15f, node.m_position.y - 1f, node.m_position.y + 1f, true, true);
            }
        }

        private void RenderRoute(RenderManager.CameraInfo cameraInfo, Vector3 start, Vector3 end, Vector3 middlePoint, Color color, float size = 0.1f)
        {
            Bezier3 bezier;
            bezier.a = start;
            bezier.d = end;
            NetSegment.CalculateMiddlePoints(bezier.a, (middlePoint - bezier.a).normalized, bezier.d, (middlePoint - bezier.d).normalized, false, false, out bezier.b, out bezier.c);

            RenderManager.instance.OverlayEffect.DrawBezier(cameraInfo, color, bezier, size, 0, 0, Mathf.Min(bezier.a.y, bezier.d.y) - 1f, Mathf.Max(bezier.a.y, bezier.d.y) + 1f, true, true);
        }
    }
}
