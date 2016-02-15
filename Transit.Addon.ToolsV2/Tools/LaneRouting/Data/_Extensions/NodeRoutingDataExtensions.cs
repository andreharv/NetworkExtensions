using UnityEngine;

namespace Transit.Addon.ToolsV2.LaneRouting.Data
{
    public static class NodeRoutingDataExtensions
    {
        public static bool IsRelevant(this NodeRoutingData data)
        {
            if (data.Routes == null || data.Routes.Count == 0)
            {
                return false;
            }

            var node = NetManager.instance.GetNode(data.NodeId);
            if (node == null)
            {
                return false;
            }

            return node.Value.CountSegments() > 1;
        }
    }
}
