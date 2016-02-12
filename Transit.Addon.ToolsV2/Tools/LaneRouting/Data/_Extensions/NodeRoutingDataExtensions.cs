namespace Transit.Addon.ToolsV2.LaneRouting.Data
{
    public static class NodeRoutingDataExtensions
    {
        public static bool IsRelevant(this NodeRoutingData data)
        {
            var node = NetManager.instance.GetNode(data.NodeId);
            return node != null && node.Value.CountSegments() > 1;
        }
    }
}
