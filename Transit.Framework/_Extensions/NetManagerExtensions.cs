namespace Transit.Framework
{
    public static class NetManagerExtensions
    {
		// TODO Delete this method if not used. It should never be called due to performance considerations.
		public static NetLane GetLane(this NetManager netManager, uint laneId)
        {
			// TODO add lane validity check & do not return a struct
			return netManager.m_lanes.m_buffer[laneId];
        }

		// TODO Delete this method if not used. It should never be called due to performance considerations.
        public static NetSegment GetLaneNetSegment(this NetManager netManager, uint laneId)
        {
			// TODO add lane, segment validity check & do not return a struct
            return netManager.m_segments.m_buffer[netManager.m_lanes.m_buffer[laneId].m_segment];
        }

        public static ushort? GetLaneNetSegmentId(this NetManager netManager, uint laneId)
        {
			if (((NetLane.Flags)netManager.m_lanes.m_buffer[laneId].m_flags & NetLane.Flags.Created) == NetLane.Flags.None)
				return null;
			ushort segmentId = netManager.m_lanes.m_buffer[laneId].m_segment;
			if ((netManager.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
				return null;
			return segmentId;
        }

        public static NetInfo GetLaneNetInfo(this NetManager netManager, uint laneId)
        {
			if (((NetLane.Flags)netManager.m_lanes.m_buffer[laneId].m_flags & NetLane.Flags.Created) == NetLane.Flags.None)
				return null;
			return netManager.m_segments.m_buffer[netManager.m_lanes.m_buffer[laneId].m_segment].Info;
        }

		/// <summary>
        /// Use with care, this method is cause perfe issues
        /// </summary>
        /// <param name="netManager"></param>
        /// <param name="laneId"></param>
        /// <returns></returns>
		public static NetInfo.Lane GetLaneInfo(this NetManager netManager, uint laneId)
        {
			if (((NetLane.Flags)netManager.m_lanes.m_buffer[laneId].m_flags & NetLane.Flags.Created) == NetLane.Flags.None)
				return null;
			ushort segmentId = netManager.m_lanes.m_buffer[laneId].m_segment;
			if ((netManager.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
				return null;
			NetInfo netInfo = netManager.m_segments.m_buffer[segmentId].Info;

            NetInfo.Lane[] netInfoLanes = netInfo.m_lanes;
            uint netSegmentLaneId = netManager.m_segments.m_buffer[segmentId].m_lanes;
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

		public static NetInfo.Lane GetLaneInfo(this NetManager netManager, ushort segmentId, byte laneIndex) {
			if ((netManager.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
				return null;
			if (laneIndex >= netManager.m_segments.m_buffer[segmentId].Info.m_lanes.Length)
				return null;
			return netManager.m_segments.m_buffer[segmentId].Info.m_lanes[laneIndex];
		}

        public static byte? GetLaneIndex(this NetManager netManager, uint laneId)
        {
			if (((NetLane.Flags)netManager.m_lanes.m_buffer[laneId].m_flags & NetLane.Flags.Created) == NetLane.Flags.None)
				return null;
			ushort segmentId = netManager.m_lanes.m_buffer[laneId].m_segment;
			if ((netManager.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
				return null;
			var netInfo = netManager.m_segments.m_buffer[segmentId].Info;

            var netInfoLanes = netInfo.m_lanes;
            var netSegmentLaneId = netManager.m_segments.m_buffer[segmentId].m_lanes;
            for (byte i = 0; i < netInfoLanes.Length && netSegmentLaneId != 0; i++)
            {
                if (netSegmentLaneId == laneId)
                {
                    return i;
                }

                netSegmentLaneId = netManager.m_lanes.m_buffer[netSegmentLaneId].m_nextLane;
            }

            return null;
        }

		// TODO Delete this method if not used. It should never be called due to performance considerations.
		public static uint? GetLaneId(this NetManager netManager, NetInfo.Lane laneInfo, ref NetSegment netSegment)
        {
            // To be tested
            NetInfo netInfo = netSegment.Info;

            NetInfo.Lane[] netInfoLanes = netInfo.m_lanes;
            uint netSegmentLaneId = netSegment.m_lanes;
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

        /// <summary>
        /// Return the node in which the lane is pointing toward
        /// </summary>
        public static ushort? FindLaneNode(this NetManager netManager, uint laneId)
        {
            var lane = netManager.m_lanes.m_buffer[laneId];

            if (((NetLane.Flags)lane.m_flags & NetLane.Flags.Created) == NetLane.Flags.None)
                return null;

            var segment = netManager.m_segments.m_buffer[lane.m_segment];
            var segmentLaneId = segment.m_lanes;
            var info = segment.Info;
            var laneCount = info.m_lanes.Length;
            var laneIndex = 0;
            for (; laneIndex < laneCount && segmentLaneId != 0; laneIndex++)
            {
                if (segmentLaneId == laneId)
                    break;
                segmentLaneId = netManager.m_lanes.m_buffer[segmentLaneId].m_nextLane;
            }

            if (laneIndex < laneCount)
            {
                NetInfo.Direction laneDir = ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ? info.m_lanes[laneIndex].m_finalDirection : NetInfo.InvertDirection(info.m_lanes[laneIndex].m_finalDirection);

                if ((laneDir & (NetInfo.Direction.Forward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Forward)
                    return segment.m_endNode;
                if ((laneDir & (NetInfo.Direction.Backward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Backward)
                    return segment.m_startNode;
            }

            return null;
        }
    }
}
