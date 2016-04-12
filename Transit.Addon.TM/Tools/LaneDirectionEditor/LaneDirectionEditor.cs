using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using Transit.Addon.TM.Overlays.LaneRouting;
using Transit.Framework;
using Transit.Framework.UI;
using UnityEngine;

namespace Transit.Addon.TM.Tools.LaneDirectionEditor
{
    public partial class LaneDirectionEditor : DefaultTool
    {
        private readonly NodeRoutesOverlay _overlay;
        private readonly Texture2D _secondPanelTexture;

        private bool _cursorIsInEditPanel;

        private ushort _hoveredNodeId;
        private ushort _hoveredSegmentId;

        private ushort _selectedNodeId;
        private ushort _selectedSegmentId;

        public LaneDirectionEditor()
        {
            _overlay = NodeRoutesOverlay.instance;
            _secondPanelTexture = TrafficManagerTool.MakeTex(1, 1, new Color(0.5f, 0.5f, 0.5f, 1f));
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

            if (Input.GetMouseButtonUp((int)MouseKeyCode.LeftButton))
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
            var mouseRayValid = !UIView.IsInsideUI() && Cursor.visible && !_cursorIsInEditPanel;
            _hoveredSegmentId = 0;
            _hoveredNodeId = 0;

            if (!mouseRayValid)
            {
                return;
            }

            // find currently hovered node
            var nodeInput = new RaycastInput(this.m_mouseRay, this.m_mouseRayLength);
            nodeInput.m_netService.m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
            nodeInput.m_netService.m_service = ItemClass.Service.Road;
            nodeInput.m_netService2.m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.PublicTransport | ItemClass.Layer.MetroTunnels;
            nodeInput.m_netService2.m_service = ItemClass.Service.PublicTransport;
            nodeInput.m_netService2.m_subService = ItemClass.SubService.PublicTransportTrain;
            nodeInput.m_ignoreTerrain = true;
            nodeInput.m_ignoreNodeFlags = NetNode.Flags.None;

            RaycastOutput nodeOutput;
            if (RayCast(nodeInput, out nodeOutput))
            {
                _hoveredNodeId = nodeOutput.m_netNode;
            }

            // find currently hovered segment
            var segmentInput = new RaycastInput(this.m_mouseRay, this.m_mouseRayLength);
            segmentInput.m_netService.m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
            segmentInput.m_netService.m_service = ItemClass.Service.Road;
            segmentInput.m_netService2.m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.PublicTransport | ItemClass.Layer.MetroTunnels;
            segmentInput.m_netService2.m_service = ItemClass.Service.PublicTransport;
            segmentInput.m_netService2.m_subService = ItemClass.SubService.PublicTransportTrain;
            segmentInput.m_ignoreTerrain = true;
            segmentInput.m_ignoreSegmentFlags = NetSegment.Flags.Untouchable;

            RaycastOutput segmentOutput;
            if (RayCast(segmentInput, out segmentOutput))
            {
                _hoveredSegmentId = segmentOutput.m_netSegment;

                if (_hoveredNodeId <= 0)
                {
                    // alternative way to get a node hit: check distance to start and end nodes of the segment
                    ushort startNodeId = Singleton<NetManager>.instance.m_segments.m_buffer[_hoveredSegmentId].m_startNode;
                    ushort endNodeId = Singleton<NetManager>.instance.m_segments.m_buffer[_hoveredSegmentId].m_endNode;

                    float startDist = (segmentOutput.m_hitPos - Singleton<NetManager>.instance.m_nodes.m_buffer[startNodeId].m_position).magnitude;
                    float endDist = (segmentOutput.m_hitPos - Singleton<NetManager>.instance.m_nodes.m_buffer[endNodeId].m_position).magnitude;
                    if (startDist < endDist && startDist < 25f)
                        _hoveredNodeId = startNodeId;
                    else if (endDist < startDist && endDist < 25f)
                        _hoveredNodeId = endNodeId;
                }
            }
        }

        private void HandleMouseClicks(InputEvent inputEvent)
        {
            if (inputEvent.MouseKeyCode == null)
            {
                return;
            }

            // Left clicks
            if (inputEvent.MouseKeyCode == MouseKeyCode.LeftButton)
            {
                if (_hoveredNodeId == 0 || _hoveredSegmentId == 0) return;

                var netFlags = Singleton<NetManager>.instance.m_nodes.m_buffer[_hoveredNodeId].m_flags;

                if ((netFlags & NetNode.Flags.Junction) == NetNode.Flags.None) return;

                if (Singleton<NetManager>.instance.m_segments.m_buffer[_hoveredSegmentId].m_startNode != _hoveredNodeId &&
                    Singleton<NetManager>.instance.m_segments.m_buffer[_hoveredSegmentId].m_endNode != _hoveredNodeId)
                    return;

                _selectedSegmentId = _hoveredSegmentId;
                _selectedNodeId = _hoveredNodeId;
            }


            // Right clicks
            if (inputEvent.MouseKeyCode == MouseKeyCode.RightButton)
            {
                _selectedSegmentId = 0;
                _selectedNodeId = 0;
            }
        }
    }
}
