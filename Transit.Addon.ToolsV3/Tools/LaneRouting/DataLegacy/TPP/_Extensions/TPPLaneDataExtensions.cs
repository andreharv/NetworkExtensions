using System.Collections.Generic;
using System.Linq;
using Transit.Addon.ToolsV3.LaneRouting.Data;

namespace Transit.Addon.ToolsV3.LaneRouting.DataLegacy.TPP
{
    public static class TPPLaneDataExtensions
    {
        public static IEnumerable<NodeRoutingData> ConvertToNodeRouting(this IEnumerable<TPPLaneData> tppData)
        {
            var groupByNodeId = tppData
                .Where(d => d != null)
                .Where(d => d.m_nodeId != 0)
                .GroupBy(d => d.m_nodeId)
                .ToArray();

            foreach (var group in groupByNodeId)
            {
                var nodeRouting = new NodeRoutingData
                {
                    NodeId = group.Key
                };
                
                foreach (var lane in group.Where(d => d.m_laneId != 0).OrderBy(d => d.m_laneId))
                {
                    if (lane.m_laneConnections != null)
                    {
                        foreach (var connection in lane.m_laneConnections.Where(c => c != 0).OrderBy(c => c))
                        {
                            var originNetLane = NetManager.instance.m_lanes.m_buffer[lane.m_laneId];
                            var originSegment = originNetLane.m_segment;

                            if (originSegment == 0)
                            {
                                continue;
                            }

                            var destNetLane = NetManager.instance.m_lanes.m_buffer[connection];
                            var destSegment = destNetLane.m_segment;

                            if (destSegment == 0)
                            {
                                continue;
                            }

                            nodeRouting.Routes.Add(new LaneRoutingData()
                            {
                                OriginSegmentId = originSegment,
                                OriginLaneId = lane.m_laneId,

                                DestinationSegmentId = destSegment,
                                DestinationLaneId = connection,
                            });
                        }
                    }
                }

                if (nodeRouting.Routes.Count > 0)
                {
                    yield return nodeRouting;
                }
            }
        }
    }
}
