namespace Transit.Addon.TrafficTools
{
    public static class NetManagerExtensions
    {
        public static NetNode? GetNode(this NetManager netManager, ushort nodeId)
        {
            if (nodeId < netManager.m_nodes.m_buffer.Length)
            {
                return netManager.m_nodes.m_buffer[nodeId];
            }

            return null;
        }
    }
}
