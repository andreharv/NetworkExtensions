using Transit.Addon.TM.Overlays.LaneRouting;
using Transit.Framework;
using Transit.Framework.UI;
using UnityEngine;

namespace Transit.Addon.TM.Tools.LaneRoutingEditor
{
    public class LaneRoutingEditor : ToolBase
    {
        private readonly NodeRoutesOverlay _overlay;
        private ushort? _hoveredNodeId;
        private ushort? _editedNodeId;

        public LaneRoutingEditor()
        {
            _overlay = NodeRoutesOverlay.instance;
        }

        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            if (m_toolController.IsInsideUI)
                return;

            var inputEvent = new InputEvent();

            if (Input.GetKeyUp(KeyCode.PageDown))
            {
                inputEvent.KeyCode = KeyCode.PageDown;
            }
            else if (Input.GetKeyUp(KeyCode.PageUp))
            {
                inputEvent.KeyCode = KeyCode.PageUp;
            }

            if (Input.GetMouseButtonUp((int) MouseKeyCode.LeftButton))
            {
                inputEvent.MouseKeyCode = MouseKeyCode.LeftButton;
            }
            else if (Input.GetMouseButtonUp((int)MouseKeyCode.RightButton))
            {
                inputEvent.MouseKeyCode = MouseKeyCode.RightButton;
            }

            inputEvent.MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            _overlay.Update(inputEvent);

            HandleMouseMoves(inputEvent);
            HandleMouseClicks(inputEvent);
        }

        private void HandleMouseMoves(InputEvent inputEvent)
        {
            _hoveredNodeId = ExtendedToolBase.RayCastNodeWithMoreThanOneSegment();

            if (_hoveredNodeId != null)
            {
                _overlay.UpdateMarker(_hoveredNodeId.Value, inputEvent);
            }

            if (_editedNodeId != null &&
                _editedNodeId != _hoveredNodeId)
            {
                _overlay.UpdateMarker(_editedNodeId.Value, inputEvent);
            }
        }

        private void HandleMouseClicks(InputEvent inputEvent)
        {
            if (inputEvent.MouseKeyCode != null)
            {
                // Left clicks
                if (inputEvent.MouseKeyCode == MouseKeyCode.LeftButton)
                {
                    if (_hoveredNodeId != null && 
                        _hoveredNodeId != _editedNodeId)
                    {
                        if (_editedNodeId != null)
                        {
                            _overlay.UnselectMarker(_editedNodeId.Value);
                            _editedNodeId = null;
                        }

                        _overlay.SelectMarker(_hoveredNodeId.Value);
                        _editedNodeId = _hoveredNodeId;
                        return;
                    }

                    if (_editedNodeId != null)
                    {
                        var marker = _overlay.GetMarker(_editedNodeId.Value);
                        if (marker != null)
                        {
                            if (marker.LeftClick())
                            {
                                return;
                            }
                        }
                    }
                }


                // Right clicks
                if (inputEvent.MouseKeyCode == MouseKeyCode.RightButton)
                {
                    if (_editedNodeId != null)
                    {
                        var marker = _overlay.GetMarker(_editedNodeId.Value);
                        if (marker != null)
                        {
                            if (marker.RightClick())
                            {
                                return; // Handled by marker
                            }
                            else
                            {
                                StopEdit();
                                return;
                            }
                        }
                    }
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _hoveredNodeId = null;
            StopEdit();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _hoveredNodeId = null;
            StopEdit();
        }

        private void StopEdit()
        {
            _overlay.UnselectCurrentMarker();
            _editedNodeId = null;
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            _overlay.Render(cameraInfo);

            if (_hoveredNodeId != null && 
                _hoveredNodeId != _editedNodeId)
            {
                NetNode node = NetManager.instance.m_nodes.m_buffer[_hoveredNodeId.Value];
                RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, new Color(0f, 0f, 0.5f, 0.75f), node.m_position, 15f, node.m_position.y - 1f, node.m_position.y + 1f, true, true);
            }
        }
    }
}
