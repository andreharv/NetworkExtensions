using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.ToolsV2.Common;
using Transit.Addon.ToolsV2.Common.Markers;
using Transit.Addon.ToolsV2.LaneRouting.Data;
using UnityEngine;

namespace Transit.Addon.ToolsV2.LaneRouting.Markers
{
    public class NodeRoutingMarker : NodeMarkerBase
    {
        private readonly NodeRoutingData _data;
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
                    throw new Exception("InitContentIfNeeded must be called before accessing the anchors");
                }

                return _anchors;
            }
        }

        public NodeRoutingMarker(ushort nodeId) : base(nodeId)
        {
            Init();
        }

        public NodeRoutingMarker(NodeRoutingData data) : base(data.NodeId)
        {
            _data = data;

            Init();

            //TEMP FOR TESTs!
            InitContentIfNeeded();
        }

        public static bool IsDataRelevant(NodeRoutingData data)
        {
            var node = NetManager.instance.GetNode(data.NodeId);
            return node != null && node.Value.CountSegments() > 1;
        }

        private void Init()
        {
            var node = NetManager.instance.GetNode(NodeId);
            IsEnabled = node != null && node.Value.CountSegments() > 1;
        }

        private bool _isContentInitialized;

        private void InitContentIfNeeded()
        {
            if (!_isContentInitialized)
            {
                InitAnchors();
                InitRoutes();

                _isContentInitialized = true;
            }
        }

        private void InitAnchors()
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

            _anchors = anchors;
        }

        private void InitRoutes()
        {
            if (_data != null)
            {
                foreach (var r in _data.Routes)
                {
                    Debug.Log(string.Format(">>>>>>>>>>>> Route: {0}.{1} -> {2}.{3}", r.OriginSegmentId, r.OriginLaneId, r.DestinationSegmentId, r.DestinationLaneId));

                    var anchors = FindAnchors(r);
                    if (anchors != null)
                    {
                        Debug.Log(">>>>>>>>>>>> anchors found");
                        ToggleRoute(anchors.Value.Key, anchors.Value.Value);
                    }
                    else
                    {
                        Debug.Log(">>>>>>>>>>>> anchors not found");
                    }
                }
            }
        }

        private KeyValuePair<LaneAnchorMarker, LaneAnchorMarker>? FindAnchors(LaneRoutingData data)
        {
            LaneAnchorMarker originAnchor = null;
            LaneAnchorMarker destinationAnchor = null;

            foreach (var a in _anchors) // FindAnchors
            {
                Debug.Log(string.Format(">>>>>>>>>>>> Anchor: {0}.{1}", a.SegmentId, a.LaneId));

                if (a.SegmentId == data.OriginSegmentId &&
                    a.LaneId == data.OriginLaneId)
                {
                    originAnchor = a;
                }

                else 
                if (a.SegmentId == data.DestinationSegmentId &&
                    a.LaneId == data.DestinationLaneId)
                {
                    destinationAnchor = a;
                }

                if (originAnchor != null && destinationAnchor != null)
                {
                    return new KeyValuePair<LaneAnchorMarker, LaneAnchorMarker>(originAnchor, destinationAnchor);
                }
            }

            return null;
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

            InitContentIfNeeded();
        }

        private void ToggleRoute(LaneAnchorMarker destination)
        {
            ToggleRoute(_editedOriginAnchor, destination);
        }

        private void ToggleRoute(LaneAnchorMarker origin, LaneAnchorMarker destination)
        {
            if (!_routes.ContainsKey(origin))
            {
                _routes[origin] = new HashSet<LaneAnchorMarker>();
            }

            if (_routes[origin].Contains(destination))
            {
                _routes[origin].Remove(destination);
            }
            else
            {
                _routes[origin].Add(destination);
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
                ClearAnchorHoverings();

                _editedOriginAnchor = null;
                ClearAnchorSelections();

                SetUnSelected();
            }
        }

        private void ClearAnchorHoverings()
        {
            foreach (var laneMarker in Anchors)
            {
                if (laneMarker.IsHovered)
                {
                    laneMarker.SetHoveringEnded();
                }
            }
        }

        private void ClearAnchorSelections()
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
            if (IsEditingRoute)
            {
                foreach (var kvp in _routes
                    .OrderBy(k => k.Key.SegmentId)
                    .Where(k => k.Key.SegmentId != _editedOriginAnchor.SegmentId))
                {
                    var marker = kvp.Key;
                    foreach (var destinationMarker in kvp.Value)
                    {
                        RenderManager.instance.OverlayEffect.DrawRouting(camera, marker.Position, destinationMarker.Position, node.m_position, marker.Color.Dim(15), ROUTE_WIDTH);
                    }
                }

                foreach (var kvp in _routes
                    .OrderBy(k => k.Key.SegmentId)
                    .Where(k => k.Key.SegmentId == _editedOriginAnchor.SegmentId)
                    .Where(k => k.Key.LaneId != _editedOriginAnchor.LaneId))
                {
                    var marker = kvp.Key;
                    foreach (var destinationMarker in kvp.Value)
                    {
                        RenderManager.instance.OverlayEffect.DrawRouting(camera, marker.Position, destinationMarker.Position, node.m_position, marker.Color, ROUTE_WIDTH);
                    }
                }

                if (_routes.ContainsKey(_editedOriginAnchor))
                {
                    var marker = _editedOriginAnchor;
                    foreach (var destinationMarker in _routes[_editedOriginAnchor])
                    {
                        RenderManager.instance.OverlayEffect.DrawRouting(camera, marker.Position, destinationMarker.Position, node.m_position, marker.Color, ROUTE_WIDTH);
                    }
                }
            }
            else
            {
                foreach (var kvp in _routes.OrderBy(k => k.Key.SegmentId))
                {
                    foreach (var destinationMarker in kvp.Value)
                    {
                        RenderManager.instance.OverlayEffect.DrawRouting(camera, kvp.Key.Position, destinationMarker.Position, node.m_position, kvp.Key.Color, ROUTE_WIDTH);
                    }
                }
            }
        }

        private const float ROUTE_WIDTH = 0.25f;
    }
}
