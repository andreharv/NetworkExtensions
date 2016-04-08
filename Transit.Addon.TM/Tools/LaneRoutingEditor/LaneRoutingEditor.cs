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
        private const NetNode.Flags CUSTOMIZED_NODE_FLAG = (NetNode.Flags)(1 << 28);
        
        private ushort? _hoveredNode;
        private ushort? _selectedNode;
        private LaneAnchorMarker _hoveredAnchor;
        private LaneAnchorMarker _selectedAnchor;
        private readonly Dictionary<ushort, IEnumerable<LaneAnchorMarker>> _nodeAnchors = new Dictionary<ushort, IEnumerable<LaneAnchorMarker>>();

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
                CreateNodeAnchors(nodeId);
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

            _hoveredNode = RayCastNodeWithMoreThanOneSegment();

            UpdateNodeAnchors(_selectedNode, _hoveredNode);

            if (Input.GetMouseButtonUp((int)MouseKeyCode.LeftButton))
            {
                if (_hoveredNode != null && _hoveredNode != _selectedNode)
                {
                    SelectNode(_hoveredNode.Value);
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
                    UnselectNode(_selectedNode.Value);
                    return;
                }
            }
        }

        private void SelectNode(ushort nodeId)
        {
            if (_selectedNode != null)
            {
                UnselectNode(_selectedNode.Value);
            }

            if (!_nodeAnchors.ContainsKey(nodeId))
            {
                CreateNodeAnchors(nodeId);
            }

            foreach (var anchor in _nodeAnchors[nodeId])
            {
                anchor.SetEnable(anchor.IsOrigin);
            }

            _selectedNode = nodeId;
        }

        private void UnselectNode(ushort nodeId)
        {
            foreach (var anchor in _nodeAnchors[nodeId])
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

            foreach (var otherAnchor in _nodeAnchors[_selectedNode.Value].Except(anchor))
            {
                otherAnchor.SetEnable(!otherAnchor.IsOrigin && otherAnchor.SegmentId != anchor.SegmentId);
            }

            _selectedAnchor = anchor;
            _selectedAnchor.Select();
        }

        private void UnselectAnchor(LaneAnchorMarker anchor)
        {
            anchor.Unselect();

            foreach (var otherAnchor in _nodeAnchors[_selectedNode.Value])
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

            if (!_nodeAnchors.ContainsKey(selectedNode.Value))
            {
                return;
            }

            var anchors = _nodeAnchors[selectedNode.Value];

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

            _hoveredNode = null;
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

        private void CreateNodeAnchors(ushort nodeId)
        {
            if (!_nodeAnchors.ContainsKey(nodeId))
            {
                _nodeAnchors[nodeId] = CreateAnchors(nodeId);
                NetManager.instance.m_nodes.m_buffer[nodeId].m_flags |= CUSTOMIZED_NODE_FLAG;
            }
        }

        private static IEnumerable<LaneAnchorMarker> CreateAnchors(ushort nodeId)
        {
            var anchors = new List<LaneAnchorMarker>();

            var node = NetManager.instance.m_nodes.m_buffer[nodeId];
            var offsetMultiplier = node.CountSegments() <= 2 ? 3 : 1;
            var segmentId = node.m_segment0;
            for (int i = 0; i < 8 && segmentId != 0; i++)
            {
                NetSegment segment = NetManager.instance.m_segments.m_buffer[segmentId];
                bool isEndNode = segment.m_endNode == nodeId;
                Vector3 offset = segment.FindDirection(segmentId, nodeId) * offsetMultiplier;
                NetInfo.Lane[] lanes = segment.Info.m_lanes;
                uint laneId = segment.m_lanes;
                for (int j = 0; j < lanes.Length && laneId != 0; j++)
                {
                    //if ((lanes[j].m_laneType & (NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle)) != NetInfo.LaneType.None)
                    if ((lanes[j].m_laneType & NetInfo.LaneType.Vehicle) == NetInfo.LaneType.Vehicle)
                    {
                        Vector3 pos = Vector3.zero;
                        NetInfo.Direction laneDir = ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ? lanes[j].m_finalDirection : NetInfo.InvertDirection(lanes[j].m_finalDirection);

                        bool isSource = false;
                        if (isEndNode)
                        {
                            if ((laneDir & (NetInfo.Direction.Forward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Forward)
                                isSource = true;
                            pos = NetManager.instance.m_lanes.m_buffer[laneId].m_bezier.d;
                        }
                        else
                        {
                            if ((laneDir & (NetInfo.Direction.Backward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Backward)
                                isSource = true;
                            pos = NetManager.instance.m_lanes.m_buffer[laneId].m_bezier.a;
                        }

                        anchors.Add(new LaneAnchorMarker(laneId, segmentId, nodeId, pos + offset, isSource, isSource? i: (int?) null));
                    }

                    laneId = NetManager.instance.m_lanes.m_buffer[laneId].m_nextLane;
                }

                segmentId = segment.GetRightSegment(nodeId);
                if (segmentId == node.m_segment0)
                    segmentId = 0;
            }

            foreach (var anchor in anchors.Where(a => a.IsOrigin))
            {
                var connections = TAMLaneRoutingManager.instance.GetLaneConnections(anchor.LaneId);
                if (connections == null || connections.Length == 0)
                    continue;

                foreach (var anchorDestination in anchors.Where(a => !a.IsOrigin))
                {
                    if (connections.Contains(anchorDestination.LaneId))
                    {
                        anchor.Connections.Add(anchorDestination);
                    }
                }
            }

            return anchors;
        }
    }
}
