namespace Transit.Addon.ToolsV3
{
    public static class NetManagerExtensions
    {
        public static NetNode? GetNode(this NetManager netManager, ushort nodeId)
        {
            if (nodeId == 0)
            {
                return null;
            }

            if (nodeId >= netManager.m_nodes.m_buffer.Length)
            {
                return null;
            }


            return netManager.m_nodes.m_buffer[nodeId];
        }

        public static NetSegment? GetSegment(this NetManager netManager, ushort segmentId)
        {
            if (segmentId == 0)
            {
                return null;
            }

            if (segmentId >= netManager.m_segments.m_buffer.Length)
            {
                return null;
            }

            var segment = netManager.m_segments.m_buffer[segmentId];

            if (segment.m_infoIndex == 0)
            {
                return null;
            }

            return segment;
        }

        public static NetLane? GetLane(this NetManager netManager, ushort laneControlBit, uint laneId)
        {
            if (laneId == 0)
            {
                return null;
            }

            if (laneId >= netManager.m_lanes.m_buffer.Length)
            {
                return null;
            }

            var lane =  netManager.m_lanes.m_buffer[laneId];

            if ((lane.m_flags & laneControlBit) == 0)
            {
                return null;
            }

            return lane;
        }

        public static void AddLaneFlag(this NetManager netManager, ushort flag, uint laneId)
        {
            if (laneId == 0)
            {
                return;
            }

            if (laneId >= netManager.m_lanes.m_buffer.Length)
            {
                return;
            }

            netManager.m_lanes.m_buffer[laneId].m_flags |= flag;
        }

        public static void RemoveLaneFlag(this NetManager netManager, ushort flag, uint laneId)
        {
            if (laneId == 0)
            {
                return;
            }

            if (laneId >= netManager.m_lanes.m_buffer.Length)
            {
                return;
            }

            netManager.m_lanes.m_buffer[laneId].m_flags &= (ushort)~flag;
        }
    }
}
