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
        private readonly Texture2D _secondPanelTexture;

        private bool _cursorInSecondaryPanel;
        private bool _mouseDown;
        private bool _mouseClicked;

        private ushort _hoveredNodeId;
        private ushort _hoveredSegmentId;

        private ushort _selectedNodeId;
        private ushort _selectedSegmentId;

        public LaneDirectionEditor()
        {
            _secondPanelTexture = TrafficManagerTool.MakeTex(1, 1, new Color(0.5f, 0.5f, 0.5f, 1f));
        }

        private void OnClickOverlay()
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

        public override void RenderGeometry(RenderManager.CameraInfo cameraInfo)
        {
            if (_hoveredNodeId != 0)
            {
                m_toolController.RenderCollidingNotifications(cameraInfo, 0, 0);
            }
        }

        /// <summary>
        /// Primarily handles click events on hovered nodes/segments
        /// </summary>
        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();
            //Log._Debug($"OnToolUpdate");

            if (Input.GetKeyUp(KeyCode.PageDown))
            {
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.Default);
                UIView.library.Hide("TrafficInfoViewPanel");
            }
            else if (Input.GetKeyUp(KeyCode.PageUp))
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);

            NodeRoutesOverlay.instance.Update(InputEvent.None);

            _mouseDown = Input.GetMouseButton(0);

            if (_mouseDown)
            {
                if (_mouseClicked) return;

                _mouseClicked = true;

                bool elementsHovered = determineHoveredElements();

                if (!elementsHovered || _cursorInSecondaryPanel)
                {
                    //Log.Message("inside ui: " + m_toolController.IsInsideUI + " visible: " + Cursor.visible + " in secondary panel: " + _cursorInSecondaryPanel);
                    return;
                }
                if (_hoveredSegmentId == 0 && _hoveredNodeId == 0)
                {
                    //Log.Message("no hovered segment");
                    return;
                }

                OnClickOverlay();
            }
            else {
                //showTooltip(false, null, Vector3.zero);
                _mouseClicked = false;
            }
        }

        public override void SimulationStep()
        {
            base.SimulationStep();

            /*currentFrame = Singleton<SimulationManager>.instance.m_currentFrameIndex >> 2;

			string displayToolTipText = tooltipText;
			if (displayToolTipText != null) {
				if (currentFrame <= tooltipStartFrame + 50) {
					ShowToolInfo(true, displayToolTipText, (Vector3)tooltipWorldPos);
				} else {
					//ShowToolInfo(false, tooltipText, (Vector3)tooltipWorldPos);
					//ShowToolInfo(false, null, Vector3.zero);
					tooltipStartFrame = 0;
					tooltipText = null;
					tooltipWorldPos = null;
				}
			}*/

            bool elementsHovered = determineHoveredElements();

            var netTool = ToolsModifierControl.toolController.Tools.OfType<NetTool>().FirstOrDefault(nt => nt.m_prefab != null);

            if (netTool != null && elementsHovered)
            {
                ToolCursor = netTool.m_upgradeCursor;
            }
        }

        private bool determineHoveredElements()
        {
            var mouseRayValid = !UIView.IsInsideUI() && Cursor.visible && !_cursorInSecondaryPanel;

            if (mouseRayValid)
            {
                _hoveredSegmentId = 0;
                _hoveredNodeId = 0;

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

                /*if (oldHoveredNodeId != HoveredNodeId || oldHoveredSegmentId != HoveredSegmentId) {
					Log._Debug($"*** Mouse ray @ node {HoveredNodeId}, segment {HoveredSegmentId}, toolMode={GetToolMode()}");
                }*/

                return (_hoveredNodeId != 0 || _hoveredSegmentId != 0);
            }
            else {
                //Log._Debug($"Mouse ray invalid: {UIView.IsInsideUI()} {Cursor.visible} {activeSubTool == null} {activeSubTool.IsCursorInPanel()}");
            }

            return mouseRayValid;
        }
    }
}
