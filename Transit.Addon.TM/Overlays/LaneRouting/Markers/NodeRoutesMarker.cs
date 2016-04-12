using System.Collections.Generic;
using System.Linq;
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
        public Vector3 Position { get; set; }

        private readonly IEnumerable<LaneAnchorMarker> _anchors;
        private LaneAnchorMarker _hoveredAnchor;
        private LaneAnchorMarker _selectedAnchor;

        public NodeRoutesMarker(ushort nodeId)
        {
            NodeId = nodeId;
            Position = NetManager.instance.m_nodes.m_buffer[nodeId].m_position;
            _anchors = new List<LaneAnchorMarker>(CreateAnchors(nodeId));
        }

        private static IEnumerable<LaneAnchorMarker> CreateAnchors(ushort nodeId)
        {
            var anchors = new List<LaneAnchorMarker>();

            var node = NetManager.instance.m_nodes.m_buffer[nodeId];
            var offsetMultiplier = node.CountSegments() <= 2 ? 3 : 1;
            var segmentId = node.m_segment0;
            for (int i = 0; i < 8 && segmentId != 0; i++)
            {
                var segment = NetManager.instance.m_segments.m_buffer[segmentId];
                var isEndNode = segment.m_endNode == nodeId;
                var offset = segment.FindDirection(segmentId, nodeId) * offsetMultiplier;
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

                        anchors.Add(new LaneAnchorMarker(laneId, segmentId, nodeId, pos + offset, isSource, isSource ? i : (int?)null));
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

        public void ToggleRoute(LaneAnchorMarker originAnchor, LaneAnchorMarker destinationAnchor)
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
                                foreach (var connection in anchor.Connections)
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

                                foreach (var connection in anchor.Connections)
                                {
                                    RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, anchor.Position, connection.Position, Position, anchor.Color, ROUTE_WIDTH);
                                }
                            }
                            else
                            {
                                foreach (var connection in anchor.Connections)
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
                        foreach (var connection in anchor.Connections)
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