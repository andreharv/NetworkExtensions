using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Framework;
using Transit.Framework.UI.Ingame;
using UnityEngine;

namespace Transit.Addon.TM.Overlays.LaneRouting.Markers
{
    public class NodeRoutesMarker : UIMarker
    {
        private const float ROUTE_WIDTH = 0.25f;

        public ushort NodeId { get; private set; }
        public Vector3 Position { get { return NetManager.instance.m_nodes.m_buffer[NodeId].m_position; } }

        private ICollection<LaneAnchorMarker> _anchors;
        private ICollection<LaneRoutesMarker> _routes;

        private LaneAnchorMarker _hoveredAnchor;
        private LaneAnchorMarker _selectedAnchor;

        public NodeRoutesMarker(ushort nodeId)
        {
            NodeId = nodeId;

            CreateAnchors();
            CreateAnchorRoutes();
        }

        private void CreateAnchors()
        {
            var anchors = new List<LaneAnchorMarker>();

            var node = NetManager.instance.m_nodes.m_buffer[NodeId];
            var offsetMultiplier = node.CountSegments() <= 2 ? 3 : 1;
            var segmentId = node.m_segment0;
            for (int i = 0; i < 8 && segmentId != 0; i++)
            {
                var segment = NetManager.instance.m_segments.m_buffer[segmentId];
                var isEndNode = segment.m_endNode == NodeId;
                var offset = segment.FindDirection(segmentId, NodeId) * offsetMultiplier;
                var lanes = segment.Info.m_lanes;
                var laneId = segment.m_lanes;
                for (int j = 0; j < lanes.Length && laneId != 0; j++)
                {
                    if ((lanes[j].m_laneType & NetInfo.LaneType.Vehicle) == NetInfo.LaneType.Vehicle)
                    {
                        var pos = Vector3.zero;
                        var laneDir = ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ? lanes[j].m_finalDirection : NetInfo.InvertDirection(lanes[j].m_finalDirection);

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

                        anchors.Add(new LaneAnchorMarker(laneId, segmentId, NodeId, pos + offset, isSource, isSource ? i : (int?)null));
                    }

                    laneId = NetManager.instance.m_lanes.m_buffer[laneId].m_nextLane;
                }

                segmentId = segment.GetRightSegment(NodeId);
                if (segmentId == node.m_segment0)
                    segmentId = 0;
            }

            _anchors = anchors.ToList();
        }

        private void CreateAnchorRoutes()
        {
            var routes = new List<LaneRoutesMarker>();

            var originAnchors = _anchors.Where(a => a.IsOrigin).ToArray();
            var destinationAnchors = _anchors.Where(a => !a.IsOrigin).ToDictionary(a => a.LaneId);

            foreach (var originAnchor in originAnchors)
            {
                var laneRoutes = TAMLaneRoutingManager.instance.GetRoute(originAnchor.LaneId);
                if (laneRoutes == null)
                    continue;

                var laneDestinations = laneRoutes
                    .Connections
                    .Select(id => 
                        destinationAnchors.ContainsKey(id) ? 
                        destinationAnchors[id] : 
                        null)
                    .Where(a => a != null)
                    .Distinct()
                    .ToArray();

                var laneRoutesMarker = new LaneRoutesMarker(laneRoutes, originAnchor, laneDestinations);
                routes.Add(laneRoutesMarker);
            }

            _routes = routes;
        }

        private LaneRoutesMarker GetLaneRoutes(LaneAnchorMarker anchor)
        {
            return _routes.FirstOrDefault(route => route.OriginArchor == anchor);
        }

        public void EnableDestinationAnchors(LaneAnchorMarker origin)
        {
            foreach (var otherAnchor in _anchors.Except(origin))
            {
                otherAnchor.SetEnable(!otherAnchor.IsOrigin && otherAnchor.SegmentId != origin.SegmentId);
            }
        }

        public void EnableOriginAnchors()
        {
            foreach (var anchor in _anchors)
            {
                anchor.SetEnable(anchor.IsOrigin);
            }
        }

        public void DisableAnchors()
        {
            foreach (var anchor in _anchors)
            {
                anchor.Disable();
            }
        }

        private void SelectAnchor(LaneAnchorMarker anchor)
        {
            if (_selectedAnchor != null)
            {
                UnselectCurrentAnchor();
            }

            EnableDestinationAnchors(anchor);

            _selectedAnchor = anchor;
            _selectedAnchor.Select();
        }

        private void UnselectCurrentAnchor()
        {
            if (_selectedAnchor != null)
            {
                _selectedAnchor.Unselect();
                _selectedAnchor = null;

                EnableOriginAnchors();
            }
        }

        public void ToggleRoute(uint laneId, NetLane.Flags direction)
        {
        }

        private void ToggleRoute(LaneAnchorMarker origin, LaneAnchorMarker destination)
        {
            if (RemoveRoute(origin, destination))
            {
                return;
            }

            if (AddRoute(origin, destination))
            {
                return;
            }
        }

        private bool AddRoute(LaneAnchorMarker origin, LaneAnchorMarker destination)
        {
            var laneRoutesMarker = GetLaneRoutes(origin);
            if (laneRoutesMarker == null)
            {
                var laneRoutes = TAMLaneRoutingManager.instance.GetOrCreateRoute(origin.LaneId);
                laneRoutesMarker = new LaneRoutesMarker(laneRoutes, origin, new LaneAnchorMarker[0]);
                _routes.Add(laneRoutesMarker);
            }

            return laneRoutesMarker.AddDestination(destination);
        }

        private bool RemoveRoute(LaneAnchorMarker origin, LaneAnchorMarker destination)
        {
            var laneRoutesMarker = GetLaneRoutes(origin);
            if (laneRoutesMarker == null)
            {
                return false;
            }

            return laneRoutesMarker.RemoveDestination(destination);
        }

        private IEnumerable<LaneAnchorMarker> GetAnchorRoutes(LaneAnchorMarker anchor)
        {
            var laneRoutes = GetLaneRoutes(anchor);
            if (laneRoutes == null)
            {
                yield break;
            }

            foreach (var destination in laneRoutes.DestinationArchors)
            {
                yield return destination;
            }
        }

        protected override bool OnLeftClick()
        {
            if (!IsSelected)
            {
                return false;
            }

            if (_hoveredAnchor != null &&
                _hoveredAnchor != _selectedAnchor)
            {
                if (_hoveredAnchor.IsOrigin)
                {
                    SelectAnchor(_hoveredAnchor);
                    return true;
                }
                else
                {
                    if (_selectedAnchor != null)
                    {
                        ToggleRoute(_selectedAnchor, _hoveredAnchor);
                        return true;
                    }
                }
            }

            return false;
        }

        protected override bool OnRightClick()
        {
            if (!IsSelected)
            {
                return false;
            }

            if (_selectedAnchor != null)
            {
                UnselectCurrentAnchor();
                return true;
            }

            return false;
        }

        protected override void OnSelected()
        {
            EnableOriginAnchors();
        }

        protected override void OnUnselected()
        {
            _hoveredAnchor = null;
            _selectedAnchor = null;
            DisableAnchors();
        }

        protected override void OnUpdate(Ray mouseRay)
        {
            if (!IsSelected)
            {
                return;
            }
            
            var bounds = new Bounds(Vector3.zero, Vector3.one);

            foreach (var anchor in _anchors.Where(a => a.IsEnabled))
            {
                bounds.center = anchor.Position;
                if (bounds.IntersectRay(mouseRay))
                {
                    anchor.Hovering();

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

        protected override void OnRendered(RenderManager.CameraInfo cameraInfo)
        {
            if (IsSelected)
            {
                if (_selectedAnchor == null)
                {
                    foreach (var anchor in _anchors)
                    {
                        if (anchor.IsOrigin)
                        {
                            if (anchor.IsEnabled)
                            {
                                foreach (var connection in GetAnchorRoutes(anchor))
                                {
                                    RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, anchor.Position, connection.Position, Position, anchor.Color, ROUTE_WIDTH);
                                }
                            }
                        }

                        anchor.Render(cameraInfo);
                    }
                }
                else
                {
                    foreach (var anchor in _anchors)
                    {
                        if (anchor.IsOrigin)
                        {
                            if (anchor == _selectedAnchor)
                            {
                                ToolBase.RaycastOutput output;
                                if (ExtendedToolBase.RayCastSegmentAndNode(out output))
                                {
                                    RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, anchor.Position, output.m_hitPos, Position, anchor.Color, ROUTE_WIDTH);
                                }

                                foreach (var connection in GetAnchorRoutes(anchor))
                                {
                                    RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, anchor.Position, connection.Position, Position, anchor.Color, ROUTE_WIDTH);
                                }
                            }
                            else
                            {
                                foreach (var connection in GetAnchorRoutes(anchor))
                                {
                                    RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, anchor.Position, connection.Position, Position, anchor.Color.Dim(75), ROUTE_WIDTH);
                                }
                            }
                        }

                        anchor.Render(cameraInfo);
                    }
                }
            }
            else
            {
                foreach (var anchor in _anchors)
                {
                    if (anchor.IsOrigin)
                    {
                        foreach (var connection in GetAnchorRoutes(anchor))
                        {
                            RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, anchor.Position, connection.Position, Position, anchor.Color, ROUTE_WIDTH);
                        }
                    }
                }
            }
        }

        public override int GetHashCode()
        {
            return NodeId.GetHashCode();
        }
    }
}