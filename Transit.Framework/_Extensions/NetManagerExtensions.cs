using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Use with care, this method is cause performance issues
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

        public static NetInfo.Lane GetLaneInfo(this NetManager netManager, ushort segmentId, byte laneIndex)
        {
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
        public static ushort? FindLaneNodeId(this NetManager netManager, uint laneId)
        {
            var lane = netManager.m_lanes.m_buffer[laneId];
            if (!lane.IsCreated())
            {
                return null;
            }

            var laneIndex = netManager.GetLaneIndex(laneId);
            if (laneIndex == null)
            {
                return null;
            }

            return netManager.FindLaneNodeId(lane.m_segment, laneIndex.Value);
        }

        /// <summary>
        /// Return the node in which the lane is pointing toward
        /// </summary>
        public static ushort? FindLaneNodeId(this NetManager netManager, ushort segmentId, byte laneIndex)
        {
            var segment = netManager.m_segments.m_buffer[segmentId];
            if (!segment.IsCreated())
            {
                return null;
            }

            var netInfo = segment.Info;
            var laneCount = netInfo.m_lanes.Length;

            if (laneIndex >= laneCount)
            {
                return null;
            }

            var laneInfo = netInfo.m_lanes[laneIndex];

            var laneDir = ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ? laneInfo.m_finalDirection : NetInfo.InvertDirection(laneInfo.m_finalDirection);

            if ((laneDir & (NetInfo.Direction.Forward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Forward)
                return segment.m_endNode;
            if ((laneDir & (NetInfo.Direction.Backward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Backward)
                return segment.m_startNode;

            return null;
        }

        public static IEnumerable<uint> GetSegmentLaneIds(this NetManager netManager, ushort segmentId)
        {
            var segment = netManager.m_segments.m_buffer[segmentId];

            if ((segment.m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
                yield break;

            var laneId = segment.m_lanes;
            var info = segment.Info;
            var laneCount = info.m_lanes.Length;
            var laneIndex = 0;
            for (; laneIndex < laneCount && laneId != 0; laneIndex++)
            {
                yield return laneId;
                laneId = netManager.m_lanes.m_buffer[laneId].m_nextLane;
            }
        }

        public static IEnumerable<NetLane> GetSegmentLanes(this NetManager netManager, ushort segmentId)
        {
            foreach (var laneId in netManager.GetSegmentLaneIds(segmentId))
            {
                yield return netManager.m_lanes.m_buffer[laneId];
            }
        }

        public static IEnumerable<NetSegment> GetSegmentsAtNode(this NetManager netManager, ushort nodeId)
        {
            var node = netManager.m_nodes.m_buffer[nodeId];

            if ((node.m_flags & NetNode.Flags.Created) == NetNode.Flags.None)
                yield break;

            foreach (var segmentId in node.GetSegmentIds())
            {
                var segment = netManager.m_segments.m_buffer[segmentId];
                if ((segment.m_flags & NetSegment.Flags.Created) != NetSegment.Flags.None)
                {
                    yield return segment;
                }
            }
        }

        public static IEnumerable<NetInfo.Lane> GetInboundLanesAtNode(this NetManager netManager, ushort segmentId, ushort nodeId)
        {
            var segment = netManager.m_segments.m_buffer[segmentId];
            var isLeftHandDrive = (segment.m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None;

            var info = segment.Info;

            var dir = segment.m_startNode == nodeId ? NetInfo.Direction.Backward : NetInfo.Direction.Forward;
            var dir2 = !isLeftHandDrive ? dir : NetInfo.InvertDirection(dir);
            var dir3 = isLeftHandDrive ? NetInfo.InvertDirection(dir2) : dir2;

            var laneIndex = 0;
            var laneId = segment.m_lanes;
            while (laneIndex < info.m_lanes.Length && laneId != 0u)
            {
                var laneInfo = info.m_lanes[laneIndex];

                if ((laneInfo.m_laneType & (NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle)) != NetInfo.LaneType.None &&
                    (laneInfo.m_vehicleType & (VehicleInfo.VehicleType.Car | VehicleInfo.VehicleType.Train)) != VehicleInfo.VehicleType.None &&
                    (laneInfo.m_direction == dir3))
                {
                    yield return laneInfo;
                }

                laneId = netManager.m_lanes.m_buffer[laneId].m_nextLane;
                laneIndex++;
            }
        }

        public static IEnumerable<uint> GetInboundLaneIdsAtNode(this NetManager netManager, ushort segmentId, ushort nodeId)
        { 
            var segment = netManager.m_segments.m_buffer[segmentId];
            var info = segment.Info;

            var laneList = new List<object[]>();

            var dir = nodeId == segment.m_startNode ? NetInfo.Direction.Backward : NetInfo.Direction.Forward;
            var finalDirection = ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ? dir : NetInfo.InvertDirection((NetInfo.Direction)dir);

            var laneId = netManager.m_segments.m_buffer[segmentId].m_lanes;
            var laneIndex = 0;
            while (laneIndex < info.m_lanes.Length && laneId != 0u)
            {
                var laneInfo = info.m_lanes[laneIndex];

                if ((laneInfo.m_laneType & (NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle)) != NetInfo.LaneType.None &&
                    (laneInfo.m_vehicleType & (VehicleInfo.VehicleType.Car | VehicleInfo.VehicleType.Train)) != VehicleInfo.VehicleType.None &&
                    (laneInfo.m_finalDirection == finalDirection))
                {
                    laneList.Add(new object[] { laneId, laneInfo.m_position, laneIndex });
                }

                laneId = netManager.m_lanes.m_buffer[laneId].m_nextLane;
                laneIndex++;
            }

            // sort lanes from left to right
            laneList.Sort(delegate (object[] x, object[] y) {
                if ((float)x[1] == (float)y[1])
                    return 0;

                if ((finalDirection == NetInfo.Direction.Forward) ^ (float)x[1] < (float)y[1])
                {
                    return 1;
                }
                return -1;
            });

            return laneList.Select(lane => (uint)lane[0]);
        }
    }
}
