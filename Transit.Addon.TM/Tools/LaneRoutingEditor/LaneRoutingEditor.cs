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
    public partial class LaneRoutingEditor : ToolBase
    {
        private ushort? _hoveredNodeId;
        private NodeRoutesMarker _selectedNode;
        private LaneAnchorMarker _hoveredAnchor;
        private LaneAnchorMarker _selectedAnchor;
        private readonly Dictionary<ushort, NodeRoutesMarker> _editedNodes = new Dictionary<ushort, NodeRoutesMarker>();

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
                _editedNodes[nodeId] = new NodeRoutesMarker(nodeId);
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
            var selectedNodeId = _selectedNode == null ? (ushort?) null : _selectedNode.NodeId;

            UpdateNodeAnchors(selectedNodeId, _hoveredNodeId);

            if (Input.GetMouseButtonUp((int)MouseKeyCode.LeftButton))
            {
                if (_hoveredNodeId != null && _hoveredNodeId != selectedNodeId)
                {
                    SelectNode(_hoveredNodeId.Value);
                    return;
                }

                if (_hoveredAnchor != null && 
                    _hoveredAnchor != _selectedAnchor)
                {
                    if (_hoveredAnchor.IsOrigin)
                    {
                        SelectAnchor(_hoveredAnchor);
                        return;
                    }
                    else
                    {
                        if (_selectedAnchor != null)
                        {
                            ToggleRoute(_selectedAnchor, _hoveredAnchor);
                            return;
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp((int)MouseKeyCode.RightButton))
            {
                if (_selectedAnchor != null)
                {
                    UnselectAnchor(_selectedAnchor);
                    return;
                }

                if (_selectedNode != null)
                {
                    UnselectNode(_selectedNode);
                    return;
                }
            }
        }

        private void SelectNode(ushort nodeId)
        {
            if (_selectedNode != null)
            {
                UnselectNode(_selectedNode);
            }

            if (!_editedNodes.ContainsKey(nodeId))
            {
                _editedNodes[nodeId] = new NodeRoutesMarker(nodeId);
            }

            SelectNode(_editedNodes[nodeId]);
        }

        private void SelectNode(NodeRoutesMarker node)
        {
            foreach (var anchor in node.Anchors)
            {
                anchor.SetEnable(anchor.IsOrigin);
            }

            _selectedNode = node;
        }

        private void UnselectNode(NodeRoutesMarker node)
        {
            foreach (var anchor in node.Anchors)
            {
                anchor.Disable();
            }

            _selectedNode = null;
        }

        private void SelectAnchor(LaneAnchorMarker anchor)
        {
            if (_selectedAnchor != null)
            {
                UnselectAnchor(_selectedAnchor);
            }

            foreach (var otherAnchor in _selectedNode.Anchors.Except(anchor))
            {
                otherAnchor.SetEnable(!otherAnchor.IsOrigin && otherAnchor.SegmentId != anchor.SegmentId);
            }

            _selectedAnchor = anchor;
            _selectedAnchor.Select();
        }

        private void UnselectAnchor(LaneAnchorMarker anchor)
        {
            anchor.Unselect();

            foreach (var otherAnchor in _selectedNode.Anchors)
            {
                otherAnchor.SetEnable(otherAnchor.IsOrigin);
            }

            _selectedAnchor = null;
        }

        private void ToggleRoute(LaneAnchorMarker originAnchor, LaneAnchorMarker destinationAnchor)
        {
            if (TAMLaneRoutingManager.instance.RemoveLaneConnection(originAnchor.LaneId, destinationAnchor.LaneId))
            {
                originAnchor.Connections.Remove(destinationAnchor);
            }
            else if (TAMLaneRoutingManager.instance.AddLaneConnection(originAnchor.LaneId, destinationAnchor.LaneId))
            {
                originAnchor.Connections.Add(destinationAnchor);
            }
        }

        private void UpdateNodeAnchors(ushort? selectedNode, ushort? hoveredNode)
        {
            if (_hoveredAnchor != null)
            {
                if (selectedNode == null || 
                    hoveredNode == null || 
                    selectedNode != hoveredNode)
                {
                    _hoveredAnchor.HoveringEnded();
                    _hoveredAnchor = null;
                }
            }

            if (selectedNode == null)
            {
                return;
            }

            if (!_editedNodes.ContainsKey(selectedNode.Value))
            {
                return;
            }

            var anchors = _editedNodes[selectedNode.Value].Anchors;

            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            var bounds = new Bounds(Vector3.zero, Vector3.one);

            foreach (var anchor in anchors.Where(a => a.IsEnabled))
            {
                bounds.center = anchor.Position;
                if (bounds.IntersectRay(mouseRay))
                {
                    if (!anchor.IsHovered)
                    {
                        anchor.HoveringStarted();
                    }
                    else
                    {
                        anchor.Hovering();
                    }

                    if (_hoveredAnchor != anchor)
                    {
                        if (_hoveredAnchor != null)
                        {
                            _hoveredAnchor.HoveringEnded();
                        }
                        _hoveredAnchor = anchor;
                    }
                }
                else
                {
                    if (anchor.IsHovered)
                    {
                        anchor.HoveringEnded();
                    }

                    if (_hoveredAnchor == anchor)
                    {
                        _hoveredAnchor = null;
                    }
                }
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            _hoveredNodeId = null;
            _selectedNode = null;
            _selectedAnchor = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_selectedAnchor != null)
            {
                _selectedAnchor.Unselect();
                _selectedAnchor = null;
            }
        }
    }
}
