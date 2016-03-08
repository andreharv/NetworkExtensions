namespace Transit.Framework
{
    public static class NetManagerExtensions
    {
        public static NetLane GetLane(this NetManager netManager, uint laneId)
        {
            return netManager.m_lanes.m_buffer[laneId];
        }

        public static NetSegment GetLaneNetSegment(this NetManager netManager, uint laneId)
        {
            var netLane = netManager.m_lanes.m_buffer[laneId];
            return netManager.m_segments.m_buffer[netLane.m_segment];
        }

        public static NetInfo GetLaneNetInfo(this NetManager netManager, uint laneId)
        {
            var netLane = netManager.m_lanes.m_buffer[laneId];
            var netSegment = netManager.m_segments.m_buffer[netLane.m_segment];
            return netSegment.Info;
        }

        public static NetInfo.Lane GetLaneInfo(this NetManager netManager, uint laneId)
        {
            // To be tested
            var netLane = netManager.m_lanes.m_buffer[laneId];
            var netSegment = netManager.m_segments.m_buffer[netLane.m_segment];
            var netInfo = netSegment.Info;

            var netInfoLanes = netInfo.m_lanes;
            var netSegmentLaneId = netSegment.m_lanes;
            for (int i = 0; i < netInfoLanes.Length && netSegmentLaneId != 0; i++)
            {
                if (netSegmentLaneId == laneId)
                {
                    return netInfoLanes[i];
                }

                netSegmentLaneId = netManager.m_lanes.m_buffer[netSegmentLaneId].m_nextLane;
            }

            return null;
        }

        public static uint? GetLaneId(this NetManager netManager, NetInfo.Lane laneInfo, NetSegment netSegment)
        {
            // To be tested
            var netInfo = netSegment.Info;

            var netInfoLanes = netInfo.m_lanes;
            var netSegmentLaneId = netSegment.m_lanes;
            for (int i = 0; i < netInfoLanes.Length && netSegmentLaneId != 0; i++)
            {
                var segmentLaneInfo = netInfoLanes[i];

                if (segmentLaneInfo == laneInfo)
                {
                    return netSegmentLaneId;
                }

                netSegmentLaneId = netManager.m_lanes.m_buffer[netSegmentLaneId].m_nextLane;
            }

            return null;
        }
    }
}
