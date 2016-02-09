using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transit.Addon.TrafficTools._Extensions
{
    public static class NetManagerHelper
    {
        public const ushort NODE_NULL = 0;
    }

    public static class NetManagerExtensions
    {
        public static NetNode? GetNode(this NetManager netManager, ushort nodeId)
        {
            if (nodeId == NetManagerHelper.NODE_NULL)
            {
                return null;
            }

            if (nodeId < netManager.m_nodes.m_buffer.Length)
            {
                return netManager.m_nodes.m_buffer[nodeId];
            }

            return null;
        }
    }
}
