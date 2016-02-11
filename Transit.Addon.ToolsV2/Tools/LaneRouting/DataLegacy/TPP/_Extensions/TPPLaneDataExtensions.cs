using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.ToolsV2.LaneRouting.Data;

namespace Transit.Addon.ToolsV2.LaneRouting.DataLegacy.TPP
{
    public static class TPPLaneDataExtensions
    {
        public static IEnumerable<NodeRoutingData> ConvertToNodeRouting(this IEnumerable<TPPLaneData> tppData)
        {
            var groupByNodeId = tppData
                .Where(d => d.m_nodeId != 0)
                .GroupBy(d => d.m_nodeId);

            foreach (var group in groupByNodeId)
            {
                var nodeRouting = new NodeRoutingData
                {
                    NodeId = @group.Key
                };

                foreach (var lane in group.Where(g => g.m_laneId != 0).OrderBy(g => g.m_laneId))
                {
                    foreach (var connection in lane.m_laneConnections.Where(c => c != 0).OrderBy(c => c))
                    {
                        nodeRouting.Routes.Add(new LaneRoutingData()
                        {
                            OriginLaneId = (ushort)lane.m_laneId,
                            DestinationLaneId = (ushort)connection,
                        });
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
