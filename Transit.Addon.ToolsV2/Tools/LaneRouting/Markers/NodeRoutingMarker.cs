using System.Collections.Generic;
using System.Linq;
using Transit.Addon.ToolsV2.Common;
using Transit.Addon.ToolsV2.Common.Markers;
using UnityEngine;

namespace Transit.Addon.ToolsV2.LaneRouting.Markers
{
    public class NodeRoutingMarker : NodeMarkerBase
    {
        private LaneAnchorMarker _hoveredAnchor;
        private LaneAnchorMarker _editedOriginAnchor;
        private readonly IDictionary<LaneAnchorMarker, ICollection<LaneAnchorMarker>> _routes = new Dictionary<LaneAnchorMarker, ICollection<LaneAnchorMarker>>();

        private IEnumerable<LaneAnchorMarker> _anchors;
        private IEnumerable<LaneAnchorMarker> Anchors
        {
            get
            {
                if (_anchors == null)
                {
                    _anchors = InitAnchors();
                }

                return _anchors;
            }
        }

        public NodeRoutingMarker(ushort nodeId) : base(nodeId)
        {
            Init();
        }

        private void Init()
        {
            var node = NetManager.instance.GetNode(NodeId);
            IsEnabled = node != null && node.Value.CountSegments() > 1;
        }

        private IEnumerable<LaneAnchorMarker> InitAnchors()
        {
            var anchors = new List<LaneAnchorMarker>();

            var allNodes = NetManager.instance.m_nodes;
            var allSegments = NetManager.instance.m_segments;
            var allLanes = NetManager.instance.m_lanes;

            var node = allNodes.m_buffer[NodeId];
            var nodeOffsetMultiplier = node.CountSegments() <= 2 ? 3 : 1;
            var segmentId = node.m_segment0;

            var anchorColorId = 0;

            for (var i = 0; i < 8 && segmentId != 0; i++)
            {
                var segment = allSegments.m_buffer[segmentId];
                var segmentOffset = segment.FindDirection(segmentId, NodeId) * nodeOffsetMultiplier;
                var isEndNode = segment.m_endNode == NodeId;
                var netInfo = segment.Info;
                var netInfoLanes = netInfo.m_lanes;
                var laneId = segment.m_lanes;

                for (var j = 0; j < netInfoLanes.Length && laneId != 0; j++)
                {
                    var lane = netInfoLanes[j];

                    if ((lane.m_laneType & NetInfo.LaneType.Vehicle) == NetInfo.LaneType.Vehicle)
                    {
                        var lanePos = Vector3.zero;
                        var laneDir =
                            ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ?
                            lane.m_finalDirection :
                            NetInfo.InvertDirection(lane.m_finalDirection);

                        var isOrigin = false;
                        if (isEndNode)
                        {
                            if ((laneDir & (NetInfo.Direction.Forward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Forward)
                                isOrigin = true;
                            lanePos = allLanes.m_buffer[laneId].m_bezier.d;
                        }
                        else
                        {
                            if ((laneDir & (NetInfo.Direction.Backward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Backward)
                                isOrigin = true;
                            lanePos = allLanes.m_buffer[laneId].m_bezier.a;
                        }

                        anchors.Add(new LaneAnchorMarker(
                            laneId,
                            segmentId,
                            lanePos + segmentOffset,
                            isOrigin,
                            isOrigin ? anchorColorId : (int?)null));
                    }

                    laneId = allLanes.m_buffer[laneId].m_nextLane;
                }

                anchorColorId++;
                segmentId = segment.GetRightSegment(NodeId);
                if (segmentId == node.m_segment0)
                    segmentId = 0;
            }

            //for (int i = 0; i < markers.m_size; i++)
            //{
            //    if (!markers.m_buffer[i].IsSource)
            //        continue;

            //    uint[] connections = LanesManager.instance.GetLaneConnections(markers.m_buffer[i].LaneId);
            //    if (connections == null || connections.Length == 0)
            //        continue;

            //    for (int j = 0; j < markers.m_size; j++)
            //    {
            //        if (markers.m_buffer[j].IsSource)
            //            continue;

            //        if (connections.Contains(markers.m_buffer[j].LaneId))
            //            markers.m_buffer[i].Connections.Add(markers.m_buffer[j]);
            //    }
            //}

            return anchors;
        }

        public override void OnHovering()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (IsSelected)
            {
                var laneMarker = GetCursorLaneMarker();

                if (_hoveredAnchor != laneMarker)
                {
                    if (_hoveredAnchor != null &&
                        _hoveredAnchor.IsHovered)
                    {
                        _hoveredAnchor.SetHoveringEnded();
                    }

                    _hoveredAnchor = laneMarker;

                    if (_hoveredAnchor != null)
                    {
                        _hoveredAnchor.SetHoveringStarted();
                    }
                }

                if (_hoveredAnchor != null)
                {
                    _hoveredAnchor.OnHovering();
                }
            }
        }

        public void OnLeftClicked()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!IsSelected)
            {
                SetSelected();
            }
            else
            {
                var laneMarker = GetCursorLaneMarker();

                if (laneMarker == null)
                {
                    return;
                }

                if (_editedOriginAnchor == laneMarker)
                {
                    return;
                }

                if (!laneMarker.IsEnabled)
                {
                    return;
                }

                if (IsEditingRoute)
                {
                    ToggleRoute(laneMarker);
                    return;
                }

                StopEditCurrentLaneMarker();

                EditLaneMarker(laneMarker);
            }
        }

        public void OnRightClicked()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (IsSelected)
            {
                if (IsEditingRoute)
                {
                    StopEditCurrentLaneMarker();
                }
                else
                {
                    UnSelect();
                }
            }
        }

        public override void SetSelected()
        {
            base.SetSelected();

            if (_anchors == null)
            {
                _anchors = InitAnchors();
            }
        }

        private void ToggleRoute(LaneAnchorMarker destination)
        {
            if (!_routes.ContainsKey(_editedOriginAnchor))
            {
                _routes[_editedOriginAnchor] = new HashSet<LaneAnchorMarker>();
            }

            if (_routes[_editedOriginAnchor].Contains(destination))
            {
                _routes[_editedOriginAnchor].Remove(destination);
            }
            else
            {
                _routes[_editedOriginAnchor].Add(destination);
            }
        }

        private LaneAnchorMarker GetCursorLaneMarker()
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            var bounds = new Bounds(Vector3.zero, Vector3.one);
            foreach (var laneMarker in Anchors)
            {
                bounds.center = laneMarker.Position;
                if (bounds.IntersectRay(mouseRay))
                {
                    return laneMarker;
                }
            }

            return null;
        }

        private void EditLaneMarker(LaneAnchorMarker laneMarkerToEdit)
        {
            _editedOriginAnchor = laneMarkerToEdit;
            _editedOriginAnchor.SetSelected();

            foreach (var laneMarker in Anchors)
            {
                if (laneMarker == _editedOriginAnchor)
                {
                    continue;
                }

                laneMarker.IsEnabled = !laneMarker.IsOrigin && laneMarker.SegmentId != _editedOriginAnchor.SegmentId;
            }
        }

        public void StopEditCurrentLaneMarker()
        {
            if (_editedOriginAnchor != null &&
                _editedOriginAnchor.IsSelected)
            {
                _editedOriginAnchor.SetUnSelected();
                _editedOriginAnchor = null;

                foreach (var laneMarker in Anchors)
                {
                    laneMarker.IsEnabled = laneMarker.IsOrigin;
                }
            }
        }

        public void UnSelect()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (IsSelected)
            {
                StopEditCurrentLaneMarker();

                _hoveredAnchor = null;
                ClearLaneHoverings();

                _editedOriginAnchor = null;
                ClearLaneSelections();

                SetUnSelected();
            }
        }

        private void ClearLaneHoverings()
        {
            foreach (var laneMarker in Anchors)
            {
                if (laneMarker.IsHovered)
                {
                    laneMarker.SetHoveringEnded();
                }
            }
        }

        private void ClearLaneSelections()
        {
            foreach (var laneMarker in Anchors)
            {
                if (laneMarker.IsSelected)
                {
                    laneMarker.SetUnSelected();
                }
            }
        }

        public override void OnRendered(RenderManager.CameraInfo camera)
        {
            var node = NetManager.instance.GetNode(NodeId);

            if (node != null)
            {
                if (!IsEnabled)
                {
                    RenderManager.instance.OverlayEffect.DrawNodeSelection(camera, node.Value, Color.red);
                    return;
                }

                if (IsSelected)
                {
                    if (IsEditingRoute)
                    {
                        RenderEditingRoute(camera, node.Value);
                    }

                    RenderRoutes(camera, node.Value);

                    foreach (var marker in Anchors)
                    {
                        marker.OnRendered(camera);
                    }
                }
                else
                {
                    RenderManager.instance.OverlayEffect.DrawNodeSelection(camera, node.Value, Color.blue);
                }
            }
        }

        private bool IsEditingRoute
        {
            get { return _editedOriginAnchor != null; }
        }

        private void RenderEditingRoute(RenderManager.CameraInfo camera, NetNode node)
        {
            var cursorPosition = TAMToolBase.GetCursorPositionInNode(NodeId);
            if (cursorPosition != null)
            {
                RenderManager.instance.OverlayEffect.DrawRouting(camera, _editedOriginAnchor.Position, cursorPosition.Value, node.m_position, _editedOriginAnchor.Color, ROUTE_WIDTH);
            }
        }

        private void RenderRoutes(RenderManager.CameraInfo camera, NetNode node)
        {
            foreach (var kvp in _routes.OrderBy(k => k.Key.SegmentId))
            {
                var originMarker = kvp.Key;

                if (_editedOriginAnchor != null && _editedOriginAnchor == originMarker)
                {
                    continue;
                }

                var drawingColor = _editedOriginAnchor != null ? originMarker.Color.Dim(30) : originMarker.Color;

                foreach (var destinationMarker in kvp.Value)
                {
                    RenderManager.instance.OverlayEffect.DrawRouting(camera, originMarker.Position, destinationMarker.Position, node.m_position, drawingColor, ROUTE_WIDTH);
                }
            }

            if (_editedOriginAnchor != null)
            {
                if (_routes.ContainsKey(_editedOriginAnchor))
                {
                    foreach (var destinationMarker in _routes[_editedOriginAnchor])
                    {
                        RenderManager.instance.OverlayEffect.DrawRouting(camera, _editedOriginAnchor.Position, destinationMarker.Position, node.m_position, _editedOriginAnchor.Color, ROUTE_WIDTH);
                    }
                }
            }
        }

        private const float ROUTE_WIDTH = 0.25f;
    }
}
