using System.Collections.Generic;
using Transit.Addon.ToolsV2.Common.Markers;
using UnityEngine;

namespace Transit.Addon.ToolsV2.Common
{
    public abstract class NetNodeEditorToolBase<TMarker> : TAMToolBase
        where TMarker : NodeMarkerBase
    {
        protected readonly IDictionary<ushort, TMarker> Markers = new Dictionary<ushort, TMarker>();
        private TMarker _hoveredMarker = null;

        private TMarker GetOrCreateMarker(ushort nodeId)
        {
            if (!Markers.ContainsKey(nodeId))
            {
                Markers[nodeId] = NodeMarker.Create<TMarker>(nodeId);
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
                else if (rightButtonClicked)
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
    }
}
