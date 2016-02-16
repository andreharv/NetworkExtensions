using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.ToolsV2.Common;
using Transit.Addon.ToolsV2.LaneRouting.Data;
using Transit.Framework.UI.Ingame;
using UnityEngine;

namespace Transit.Addon.ToolsV2.LaneRouting.Markers
{
    public class NodeRoutingMarker : UIMarker
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

        public NodeRoutingMarker(NodeRoutingData data)
        {
            _data = data;

            Init();
        }

        private void Init()
        {
            var node = NetManager.instance.GetNode(_data.NodeId);
            if (node == null || node.Value.CountSegments() <= 1)
            {
                Disable();
            }
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

            var node = allNodes.m_buffer[_data.NodeId];
            var nodeOffsetMultiplier = node.CountSegments() <= 2 ? 3 : 1;
            var segmentId = node.m_segment0;

            var anchorColorId = 0;

            for (var i = 0; i < 8 && segmentId != 0; i++)
            {
                var segment = allSegments.m_buffer[segmentId];
                var segmentOffset = segment.FindDirection(segmentId, _data.NodeId) * nodeOffsetMultiplier;
                var isEndNode = segment.m_endNode == _data.NodeId;
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
                segmentId = segment.GetRightSegment(_data.NodeId);
                if (segmentId == node.m_segment0)
                    segmentId = 0;
            }

            _anchors = anchors;
        }

        private void InitRoutes()
        {
            foreach (var r in _data.Routes)
            {
                var anchors = FindAnchors(r);
                if (anchors != null)
                {
                    ToggleRoute(anchors.Value.Key, anchors.Value.Value, true);
                }
            }
        }

        private KeyValuePair<LaneAnchorMarker, LaneAnchorMarker>? FindAnchors(LaneRoutingData data)
        {
            LaneAnchorMarker originAnchor = null;
            LaneAnchorMarker destinationAnchor = null;

            foreach (var a in _anchors) // FindAnchors is being call on init, dont replace by Anchors
            {
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

        protected override void OnHovering()
        {
            if (IsSelected)
            {
                var laneMarker = GetCursorLaneMarker();

                if (_hoveredAnchor != laneMarker)
                {
                    if (_hoveredAnchor != null)
                    {
                        _hoveredAnchor.HoveringEnded();
                    }

                    _hoveredAnchor = laneMarker;

                    if (_hoveredAnchor != null)
                    {
                        _hoveredAnchor.HoveringStarted();
                    }
                }

                if (_hoveredAnchor != null)
                {
                    _hoveredAnchor.Hovering();
                }
            }
        }

        public void LeftClick()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!IsSelected)
            {
                Select();
                return;
            }

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

        public void RightClick()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!IsSelected)
            {
                return;
            }

            if (IsEditingRoute)
            {
                StopEditCurrentLaneMarker();
            }
            else
            {
                Unselect();
            }
        }

        protected override void OnSelected()
        {
            base.OnSelected();

            InitContentIfNeeded();
        }

        private void ToggleRoute(LaneAnchorMarker destination)
        {
            ToggleRoute(_editedOriginAnchor, destination);
        }

        private void ToggleRoute(LaneAnchorMarker origin, LaneAnchorMarker destination, bool isInitializing = false)
        {
            if (!_routes.ContainsKey(origin))
            {
                _routes[origin] = new HashSet<LaneAnchorMarker>();
            }

            if (_routes[origin].Contains(destination))
            {
                _routes[origin].Remove(destination);
                if (!isInitializing)
                {
                    OnRouteRemoving(origin, destination);
                }
            }
            else
            {
                _routes[origin].Add(destination);
                if (!isInitializing)
                {
                    OnRouteAdding(origin, destination);
                }
            }
        }

        private void OnRouteAdding(LaneAnchorMarker origin, LaneAnchorMarker destination)
        {
            _data.AddRoute(new LaneRoutingData
            {
                OriginSegmentId = origin.SegmentId,
                OriginLaneId = origin.LaneId,
                DestinationSegmentId = destination.SegmentId,
                DestinationLaneId = destination.LaneId,
            });
        }

        private void OnRouteRemoving(LaneAnchorMarker origin, LaneAnchorMarker destination)
        {
            _data.RemoveRoute(new LaneRoutingData
            {
                OriginSegmentId = origin.SegmentId,
                OriginLaneId = origin.LaneId,
                DestinationSegmentId = destination.SegmentId,
                DestinationLaneId = destination.LaneId,
            });
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
            _editedOriginAnchor.Select();

            foreach (var laneMarker in Anchors)
            {
                if (laneMarker == _editedOriginAnchor)
                {
                    continue;
                }

                laneMarker.SetEnable(!laneMarker.IsOrigin && laneMarker.SegmentId != _editedOriginAnchor.SegmentId);
            }
        }

        public void StopEditCurrentLaneMarker()
        {
            if (_editedOriginAnchor != null &&
                _editedOriginAnchor.IsSelected)
            {
                _editedOriginAnchor.Unselect();
                _editedOriginAnchor = null;

                foreach (var laneMarker in Anchors)
                {
                    laneMarker.SetEnable(laneMarker.IsOrigin);
                }
            }
        }

        protected override void OnUnselected()
        {
            base.OnUnselected();

            StopEditCurrentLaneMarker();

            _hoveredAnchor = null;
            ClearAnchorHoverings();

            _editedOriginAnchor = null;
            ClearAnchorSelections();
        }

        private void ClearAnchorHoverings()
        {
            foreach (var laneMarker in Anchors)
            {
                laneMarker.HoveringEnded();
            }
        }

        private void ClearAnchorSelections()
        {
            foreach (var laneMarker in Anchors)
            {
                if (laneMarker.IsSelected)
                {
                    laneMarker.Unselect();
                }
            }
        }

        protected override void OnRendered(RenderManager.CameraInfo camera)
        {
            var node = NetManager.instance.GetNode(_data.NodeId);

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
                        marker.Render(camera);
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
            var cursorPosition = TAMToolBase.GetCursorPositionInNode(_data.NodeId);
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
