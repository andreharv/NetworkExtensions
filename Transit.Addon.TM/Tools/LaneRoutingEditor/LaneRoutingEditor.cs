using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Addon.TM.Tools.LaneRoutingEditor.Markers;
using Transit.Framework;
using Transit.Framework.UI;
using UnityEngine;

namespace Transit.Addon.TM.Tools.LaneRoutingEditor
{
    public class LaneRoutingEditor : ExtendedToolBase
    {
        private ushort? _hoveredNodeId;
        private NodeRoutesMarker _selectedNodeMarker;
        private readonly Dictionary<ushort, NodeRoutesMarker> _nodeMarkers = new Dictionary<ushort, NodeRoutesMarker>();

        protected override void Awake()
        {
            base.Awake();

            StartCoroutine(LoadMarkers());
        }

        private IEnumerator LoadMarkers()
        {
            while (!TAMLaneRoutingManager.instance.IsLoaded())
            {
                yield return new WaitForEndOfFrame();
            }

            var nodesList = new HashSet<ushort>();
            foreach (var route in TAMLaneRoutingManager.instance.GetAllRoutes())
            {
                if (route == null)
                    continue;

                if (route.Connections.Any())
                    nodesList.Add(route.NodeId);
            }

            foreach (var nodeId in nodesList)
            {
                _nodeMarkers[nodeId] = new NodeRoutesMarker(nodeId);
            }
        }

        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            if (Input.GetKeyUp(KeyCode.PageDown))
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.Default);
            else if (Input.GetKeyUp(KeyCode.PageUp))
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);

            if (m_toolController.IsInsideUI)
                return;

            _hoveredNodeId = RayCastNodeWithMoreThanOneSegment();
            var selectedNodeId = _selectedNodeMarker == null ? (ushort?) null : _selectedNodeMarker.NodeId;


            // -------------------------------------------------
            // Mouse moves
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (_hoveredNodeId != null)
            {
                if (_nodeMarkers.ContainsKey(_hoveredNodeId.Value))
                {
                    _nodeMarkers[_hoveredNodeId.Value].Update(mouseRay);
                }
            }

            if (_selectedNodeMarker != null &&
                _hoveredNodeId != selectedNodeId)
            {
                _selectedNodeMarker.Update(mouseRay);
            }


            // -------------------------------------------------
            // Mouse left clicks
            if (Input.GetMouseButtonUp((int)MouseKeyCode.LeftButton))
            {
                if (_hoveredNodeId != null && _hoveredNodeId != selectedNodeId)
                {
                    if (_selectedNodeMarker != null)
                    {
                        _selectedNodeMarker.Unselect();
                        _selectedNodeMarker = null;
                    }

                    if (!_nodeMarkers.ContainsKey(_hoveredNodeId.Value))
                    {
                        _nodeMarkers[_hoveredNodeId.Value] = new NodeRoutesMarker(_hoveredNodeId.Value);
                    }

                    var node = _nodeMarkers[_hoveredNodeId.Value];

                    _selectedNodeMarker = node;
                    _selectedNodeMarker.Select();
                    return;
                }

                if (_selectedNodeMarker != null)
                {
                    if (_selectedNodeMarker.LeftClick())
                    {
                        return;
                    }
                }
            }


            // -------------------------------------------------
            // Mouse right clicks
            if (Input.GetMouseButtonUp((int)MouseKeyCode.RightButton))
            {
                if (_selectedNodeMarker != null)
                {
                    if (_selectedNodeMarker.RightClick())
                    {
                        return;
                    }
                }

                if (_selectedNodeMarker != null)
                {
                    _selectedNodeMarker.Unselect();
                    _selectedNodeMarker = null;
                    return;
                }
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            Reset();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Reset();
        }

        private void Reset()
        {
            _hoveredNodeId = null;

            if (_selectedNodeMarker != null)
            {
                _selectedNodeMarker.Unselect();
                _selectedNodeMarker = null;
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            if (_selectedNodeMarker != null)
            {
                _selectedNodeMarker.Render(cameraInfo);
            }
            else
            {
                foreach (var kvp in _nodeMarkers)
                {
                    kvp.Value.Render(cameraInfo);
                }
            }

            var selectedNodeId = _selectedNodeMarker == null ? (ushort?)null : _selectedNodeMarker.NodeId;
            if (_hoveredNodeId != null && _hoveredNodeId != selectedNodeId)
            {
                NetNode node = NetManager.instance.m_nodes.m_buffer[_hoveredNodeId.Value];
                RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, new Color(0f, 0f, 0.5f, 0.75f), node.m_position, 15f, node.m_position.y - 1f, node.m_position.y + 1f, true, true);
            }
        }
    }
}
