using ColossalFramework.Math;
using System.Collections.Generic;
using Transit.Addon.TrafficTools.Common.Markers;
using UnityEngine;

namespace Transit.Addon.TrafficTools.Common
{
    public abstract class NetNodeEditorToolBase<TMarker> : ToolBase
        where TMarker : NetNodeMarkerBase
    {
        protected readonly IDictionary<ushort, TMarker> Markers = new Dictionary<ushort, TMarker>();
        private TMarker _hoveredMarker = null;

        private TMarker GetOrCreateMarker(ushort nodeId)
        {
            if (!Markers.ContainsKey(nodeId))
            {
                Markers[nodeId] = NetNodeMarker.Create<TMarker>(nodeId);
            }

            return Markers[nodeId];
        }

        protected override void OnToolUpdate()
        {
            // Toggle underground view
            if (Input.GetKeyUp(KeyCode.PageDown))
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.Default);
            else if (Input.GetKeyUp(KeyCode.PageUp))
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);

            if (this.m_toolController.IsInsideUI)
                return;

            var nodeId = GetCursorNode();

            var leftButtonClicked = Input.GetMouseButtonUp((int)MouseKeyCode.LeftButton);
            var rightButtonClicked = Input.GetMouseButtonUp((int)MouseKeyCode.RightButton);

            if (nodeId == null)
            {
                if (_hoveredMarker != null)
                {
                    OnMarkerHoveringEnded(_hoveredMarker);
                    _hoveredMarker = null;
                }

                if (leftButtonClicked)
                {
                    OnNonMarkerClicked(MouseKeyCode.LeftButton);
                }
                else // if (rightButtonClicked)
                {
                    OnNonMarkerClicked(MouseKeyCode.RightButton);
                }
                return;
            }

            var isHovering = !leftButtonClicked && !rightButtonClicked;
            if (isHovering)
            {
                var nodeMarker = GetOrCreateMarker(nodeId.Value);

                if (_hoveredMarker != nodeMarker)
                {
                    if (_hoveredMarker != null)
                    {
                        OnMarkerHoveringEnded(_hoveredMarker);
                    }
                    _hoveredMarker = nodeMarker;
                    OnMarkerHoveringStarted(_hoveredMarker);
                }

                OnMarkerHovering(_hoveredMarker);

                return;
            }

            if (leftButtonClicked)
            {
                if (_hoveredMarker != null)
                {
                    OnMarkerClicked(_hoveredMarker, MouseKeyCode.LeftButton);
                }
            }
            else // if (rightButtonClicked)
            {
                if (_hoveredMarker != null)
                {
                    OnMarkerClicked(_hoveredMarker, MouseKeyCode.RightButton);
                }
            }
        }

        protected virtual void OnMarkerHoveringStarted(TMarker marker)
        {
            marker.SetHoveringStarted();
        }

        protected virtual void OnMarkerHoveringEnded(TMarker marker)
        {
            marker.SetHoveringEnded();
        }

        protected virtual void OnMarkerHovering(TMarker marker)
        {
            marker.OnHovering();
        }

        protected virtual void OnMarkerClicked(TMarker marker, MouseKeyCode code)
        {
        }

        protected virtual void OnNonMarkerClicked(MouseKeyCode code)
        {
        }

        public override void RenderOverlay(RenderManager.CameraInfo camera)
        {
            base.RenderOverlay(camera);

            if (_hoveredMarker != null)
            {
                _hoveredMarker.OnRendered(camera);
            }
        }

        protected static ushort? GetCursorNode()
        {
            RaycastInput raycastInput = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane);
            raycastInput.m_netService.m_service = ItemClass.Service.Road;
            raycastInput.m_netService.m_subService = ItemClass.SubService.None;
            raycastInput.m_netService.m_itemLayers = ItemClass.Layer.Default;
            raycastInput.m_ignoreNodeFlags = NetNode.Flags.None;
            raycastInput.m_ignoreSegmentFlags = NetSegment.Flags.None;
            raycastInput.m_ignoreTerrain = true;

            RaycastOutput output;
            ushort nodeId;

            if (!RayCast(raycastInput, out output))
            {
                return null;
            }

            nodeId = output.m_netNode;

            if (nodeId == 0)
            {
                // Joao Farias: I tried caching the raycast input, since it always has the same properties, but it causes weird issues
                NetManager netManager = NetManager.instance;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                NetSegment seg = netManager.m_segments.m_buffer[output.m_netSegment];

                ushort[] segNodes = new ushort[] { seg.m_startNode, seg.m_endNode };
                for (int i = 0; i < segNodes.Length; i++)
                {
                    Bounds bounds = netManager.m_nodes.m_buffer[segNodes[i]].m_bounds;
                    if (bounds.IntersectRay(ray))
                    {
                        nodeId = segNodes[i];
                        break;
                    }
                }
            }

            if (nodeId == 0)
            {
                return null;
            }
            else
            {
                return nodeId;
            }
        }

        protected bool RaycastBezier(Bezier3 bezier)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            float angle = Vector3.Angle(bezier.a, bezier.b);
            if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
            {
                angle = Vector3.Angle(bezier.b, bezier.c);
                if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
                {
                    angle = Vector3.Angle(bezier.c, bezier.d);
                    if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
                    {
                        // linear bezier
                        Bounds bounds = bezier.GetBounds();
                        bounds.Expand(0.5f);

                        return bounds.IntersectRay(mouseRay);
                    }
                }
            }

            // split bezier in 10 parts to correctly raycast curves
            const int amount = 10;
            const float size = 1f / amount;
            Bezier3 tempBezier;
            for (int i = 0; i < amount; i++)
            {
                tempBezier = bezier.Cut(i * size, (i + 1) * size);

                Bounds bounds = tempBezier.GetBounds();
                bounds.Expand(0.5f);

                if (bounds.IntersectRay(mouseRay))
                    return true;
            }

            return false;
        }
    }
}
