using System.Collections.Generic;
using System.Linq;
using Transit.Addon.TM.PathFindingFeatures;
using UnityEngine;

namespace Transit.Addon.TM.Tools.LaneRoutingEditor.Markers
{
    public class NodeRoutesMarker
    {
        public ushort NodeId { get; private set; }
        public Vector3 Position { get; set; }
        public ICollection<LaneAnchorMarker> Anchors { get; private set; }

        public NodeRoutesMarker(ushort nodeId)
        {
            NodeId = nodeId;
            Position = NetManager.instance.m_nodes.m_buffer[nodeId].m_position;
            Anchors = new List<LaneAnchorMarker>(CreateAnchors(nodeId));
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

        public override int GetHashCode()
        {
            return NodeId.GetHashCode();
        }
    }
}