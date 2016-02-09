using System.Collections.Generic;
using Transit.Addon.TrafficTools.Common;
using Transit.Addon.TrafficTools.Common.Markers;
using UnityEngine;

namespace Transit.Addon.TrafficTools.LaneRouting.Markers
{
    public class NetNodeRoutingMarker : NetNodeMarkerBase
    {
        private IEnumerable<NetLaneAnchorMarker> _laneMarkers;

        public IEnumerable<NetLaneAnchorMarker> LaneMarkers
        {
            get
            {
                if (_laneMarkers == null)
                {
                    Init();
                }

                return _laneMarkers;
            }
        }

        //private NodeLaneRoutingMarker _selectedMarker;

        private void Init()
        {
            var markers = new List<NetLaneAnchorMarker>();

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

                        markers.Add(new NetLaneAnchorMarker(
                            laneId,
                            lanePos + segmentOffset,
                            isOrigin,
                            isOrigin ? anchorColorId : anchorColorId + 1));
                    }

                    laneId = allLanes.m_buffer[laneId].m_nextLane;
                }

                anchorColorId += 2;
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

            _laneMarkers = markers;


            IsEnabled = node.CountSegments() > 1;
        }

        public NetNodeRoutingMarker(ushort nodeId) : base(nodeId)
        {
            
        }

        public override void OnHovering()
        {
            base.OnHovering();

            if (IsSelected)
            {
                var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                var bounds = new Bounds(Vector3.zero, Vector3.one);
                foreach (var laneMarker in LaneMarkers)
                {
                    bounds.center = laneMarker.Position;
                    if (bounds.IntersectRay(mouseRay))
                    {
                        if (!laneMarker.IsHovered)
                        {
                            laneMarker.SetHoveringStarted();
                        }
                    }
                    else
                    {
                        if (laneMarker.IsHovered)
                        {
                            laneMarker.SetHoveringEnded();
                        }
                    }
                }
            }
        }

        public override void Select()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!IsSelected)
            {
                base.Select();
            }
        }

        public override void UnSelect()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (IsSelected)
            {
                base.UnSelect();
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
                    foreach (var marker in LaneMarkers)
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
    }
}
