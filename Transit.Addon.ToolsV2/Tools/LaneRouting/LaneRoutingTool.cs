using System.Collections.Generic;
using Transit.Addon.ToolsV2.Common;
using Transit.Addon.ToolsV2.LaneRouting.Data;
using Transit.Addon.ToolsV2.LaneRouting.Markers;
using Transit.Framework.UI;
using UnityEngine;

namespace Transit.Addon.ToolsV2.LaneRouting
{
    public class LaneRoutingTool : TAMToolBase
    {
        private readonly IDictionary<ushort, NodeRoutingMarker> _markers = new Dictionary<ushort, NodeRoutingMarker>();
        private NodeRoutingMarker _selectedMarker;
        private NodeRoutingMarker _hoveredMarker = null;

        public LaneRoutingTool()
        {
            OnInit();
        }

        private void OnInit()
        {
            foreach (var d in LaneRoutingManager.GetAllData())
            {
                d.UpdateArrows(); // Lets force an arrow update on load
                _markers[d.NodeId] = new NodeRoutingMarker(d);
            }
        }

        private NodeRoutingMarker GetOrCreateMarker(ushort nodeId)
        {
            if (!_markers.ContainsKey(nodeId))
            {
                _markers[nodeId] = new NodeRoutingMarker(LaneRoutingManager.GetOrCreateData(nodeId));
            }

            return _markers[nodeId];
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

        private void OnMarkerHoveringStarted(NodeRoutingMarker marker)
        {
            marker.HoveringStarted();
        }

        private void OnMarkerHoveringEnded(NodeRoutingMarker marker)
        {
            marker.HoveringEnded();
        }

        private void OnMarkerHovering(NodeRoutingMarker marker)
        {
            marker.Hovering();
        }

        private void OnMarkerClicked(NodeRoutingMarker marker, MouseKeyCode code)
        {
            switch (code)
            {
                case MouseKeyCode.LeftButton:
                    if (_selectedMarker != marker)
                    {
                        if (_selectedMarker != null)
                        {
                            _selectedMarker.Unselect();
                            _selectedMarker = null;
                        }
                    }

                    if (marker.IsEnabled)
                    {
                        _selectedMarker = marker;
                        _selectedMarker.LeftClick();
                    }
                    break;

                case MouseKeyCode.RightButton:
                    if (_selectedMarker != marker)
                    {
                        if (_selectedMarker != null)
                        {
                            _selectedMarker.Unselect();
                            _selectedMarker = null;
                        }
                    }
                    else
                    {
                        if (_selectedMarker != null)
                        {
                            _selectedMarker.RightClick();
                        }
                    }
                    break;
            }
        }

        private void OnNonMarkerClicked(MouseKeyCode code)
        {
            if (_selectedMarker != null)
            {
                _selectedMarker.Unselect();
                _selectedMarker = null;
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo camera)
        {
            base.RenderOverlay(camera);

            if (_hoveredMarker != null)
            {
                _hoveredMarker.Render(camera);
            }

            if (_selectedMarker != null &&
                _selectedMarker != _hoveredMarker)
            {
                _selectedMarker.Render(camera);
            }
        }
    }
}
