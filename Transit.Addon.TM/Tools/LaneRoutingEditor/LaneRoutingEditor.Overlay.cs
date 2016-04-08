using ColossalFramework.Math;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.TM.Tools.LaneRoutingEditor
{
    public partial class LaneRoutingEditor
    {
        private const float ROUTE_WIDTH = 0.25f;

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            if (_selectedNode != null)
            {
                var nodePos = NetManager.instance.m_nodes.m_buffer[_selectedNode.Value].m_position;

                if (_selectedAnchor == null)
                {
                    foreach (var anchor in _nodeAnchors[_selectedNode.Value])
                    {
                        if (anchor.IsOrigin)
                        {
                            if (anchor.IsEnabled)
                            {
                                foreach (var connection in anchor.Connections)
                                {
                                    RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, anchor.Position, connection.Position, nodePos, anchor.Color, ROUTE_WIDTH);
                                }
                            }
                        }

                        anchor.Render(cameraInfo);
                    }
                }
                else
                {
                    foreach (var anchor in _nodeAnchors[_selectedNode.Value])
                    {
                        if (anchor.IsOrigin)
                        {
                            if (anchor == _selectedAnchor)
                            {
                                RaycastOutput output;
                                if (RayCastSegmentAndNode(out output))
                                {
                                    RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, anchor.Position, output.m_hitPos, nodePos, anchor.Color, ROUTE_WIDTH);
                                }

                                foreach (var connection in anchor.Connections)
                                {
                                    RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, anchor.Position, connection.Position, nodePos, anchor.Color, ROUTE_WIDTH);
                                }
                            }
                            else
                            {
                                foreach (var connection in anchor.Connections)
                                {
                                    RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, anchor.Position, connection.Position, nodePos, anchor.Color.Dim(75), ROUTE_WIDTH);
                                }
                            }
                        }

                        anchor.Render(cameraInfo);
                    }
                }
            }
            else
            {
                foreach (var kvp in _nodeAnchors)
                {
                    var nodePos = NetManager.instance.m_nodes.m_buffer[kvp.Key].m_position;

                    foreach (var anchor in kvp.Value)
                    {
                        if (anchor.IsOrigin)
                        {
                            foreach (var connection in anchor.Connections)
                            {
                                RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, anchor.Position, connection.Position, nodePos, anchor.Color, ROUTE_WIDTH);
                            }
                        }
                    }
                }
            }

            if (_hoveredNode != null && _hoveredNode != _selectedNode)
            {
                NetNode node = NetManager.instance.m_nodes.m_buffer[_hoveredNode.Value];
                RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, new Color(0f, 0f, 0.5f, 0.75f), node.m_position, 15f, node.m_position.y - 1f, node.m_position.y + 1f, true, true);
            }
        }
    }
}
