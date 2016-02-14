using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transit.Addon.ToolsV2.LaneRouting.Data;
using UnityEngine;

namespace Transit.Addon.ToolsV2.LaneRouting.Core
{
    public partial class RoutingDataManager : SerializableDataExtensionBase
    {
        private const ushort LANE_CONTROL_BIT = 2048;

        private static IDictionary<uint, NodeRoutingData> _routingData;

        public static IEnumerable<NodeRoutingData> GetAllData()
        {
            if (_routingData == null)
            {
                throw new Exception("Routing has not been initialized/loaded yet");
            }

            return _routingData.Values;
        }

        public static NodeRoutingData GetOrCreateData(ushort nodeId)
        {
            if (_routingData == null)
            {
                throw new Exception("Routing has not been initialized/loaded yet");
            }

            if (!_routingData.ContainsKey(nodeId))
            {
                _routingData[nodeId] = new NodeRoutingData { NodeId = nodeId };
            }

            return _routingData[nodeId];
        }

        public static void AddRoute(NodeRoutingData nodeRouting, LaneRoutingData route)
        {
            if (!nodeRouting.Routes.Contains(route))
            {
                lock (nodeRouting.Routes)
                {
                    nodeRouting.Routes.Add(route);
                    NetManager.instance.AddLaneFlag(LANE_CONTROL_BIT, route.OriginLaneId);
                }
            }

            UpdateArrows(nodeRouting);
        }

        public static void RemoveRoute(NodeRoutingData nodeRouting, LaneRoutingData route)
        {
            if (nodeRouting.Routes.Contains(route))
            {
                lock (nodeRouting.Routes)
                {
                    nodeRouting.Routes.Remove(route);

                    if (nodeRouting.Routes.Count == 0)
                    {
                        NetManager.instance.RemoveLaneFlag(LANE_CONTROL_BIT, route.OriginLaneId);
                    }
                }
            }

            UpdateArrows(nodeRouting);
        }

        public static void VerifyRoutes(NodeRoutingData nodeRouting)
        {
            lock (nodeRouting.Routes)
            {
                ICollection<LaneRoutingData> obsoleteRoutes = null;

                foreach (var route in nodeRouting.Routes)
                {
                    var originLane = NetManager.instance.GetLane(LANE_CONTROL_BIT, route.OriginLaneId);
                    if (originLane == null)
                    {
                        if (obsoleteRoutes == null)
                        {
                            obsoleteRoutes = new HashSet<LaneRoutingData>();
                        }
                        obsoleteRoutes.Add(route);
                        continue;
                    }

                    var destinationLane = NetManager.instance.GetLane(LANE_CONTROL_BIT, route.DestinationLaneId);
                    if (destinationLane == null)
                    {
                        if (obsoleteRoutes == null)
                        {
                            obsoleteRoutes = new HashSet<LaneRoutingData>();
                        }
                        obsoleteRoutes.Add(route);
                        continue;
                    }
                }

                if (obsoleteRoutes != null)
                {
                    foreach (var route in obsoleteRoutes)
                    {
                        nodeRouting.Routes.Remove(route);
                    }
                }
            }
        }

        public static void UpdateArrows(NodeRoutingData nodeRouting)
        {
            var node = NetManager.instance.GetNode(nodeRouting.NodeId);
            if (node == null)
            {
                return;
            }

            SetDefaultArrows(nodeRouting);
            SetRoutedArrows(nodeRouting);
        }

        public static void SetDefaultArrows(NodeRoutingData nodeRouting)
        {
            var node = NetManager.instance.GetNode(nodeRouting.NodeId);
            if (node == null)
            {
                return;
            }

            foreach (var segmentId in node.Value.GetSegments())
            {
                var segment = NetManager.instance.GetSegment(segmentId);
                if (segment == null)
                {
                    continue;
                }

                var segmentValue = segment.Value;
                var info = segmentValue.Info;
                info.m_netAI.UpdateLanes(segmentId, ref segmentValue, false);
            }
        }

        public static void SetRoutedArrows(NodeRoutingData nodeRouting)
        {
            var node = NetManager.instance.GetNode(nodeRouting.NodeId);
            if (node == null)
            {
                return;
            }

            foreach (var group in nodeRouting.Routes.GroupBy(r => r.GetOriginUniqueId()))
            {
                var originSegmentId = group.First().OriginSegmentId;
                var originSegment = NetManager.instance.GetSegment(originSegmentId);
                if (originSegment == null)
                {
                    continue;
                }

                var originLaneId = group.First().OriginLaneId;
                var originLane = NetManager.instance.GetLane(LANE_CONTROL_BIT, originLaneId);
                if (originLane == null)
                {
                    continue;
                }

                var originSegmentDirection = originSegment.Value.GetDirection(nodeRouting.NodeId);
                var originLaneArrowFlags = NetLane.Flags.None;
                
                foreach (var route in group)
                {
                    var destSegmentId = route.DestinationSegmentId;
                    var destSegment = NetManager.instance.GetSegment(destSegmentId);
                    if (destSegment == null)
                    {
                        continue;
                    }

                    var destSegmentDirection = destSegment.Value.GetDirection(nodeRouting.NodeId);
                    if (Vector3.Angle(originSegmentDirection, destSegmentDirection) > 150f)
                    {
                        originLaneArrowFlags |= NetLane.Flags.Forward;
                    }
                    else
                    {
                        if (Vector3.Dot(Vector3.Cross(originSegmentDirection, -destSegmentDirection), Vector3.up) > 0f)
                        {
                            originLaneArrowFlags |= NetLane.Flags.Right;
                        }
                        else
                        {
                            originLaneArrowFlags |= NetLane.Flags.Left;
                        }
                    }
                }

                if (originLaneArrowFlags != NetLane.Flags.None)
                {
                    NetManager.instance.m_lanes.m_buffer[originLaneId].m_flags = 
                        (ushort)(
                            ((NetLane.Flags)originLane.Value.m_flags &
                            ~NetLane.Flags.LeftForwardRight) |
                            originLaneArrowFlags);
                }
            }

            //if (ConnectionCount() == 0)
            //{
            //    SetDefaultArrows(lane.m_segment, ref NetManager.instance.m_segments.m_buffer[lane.m_segment]);
            //    return;
            //}

            //NetLane.Flags flags = (NetLane.Flags)lane.m_flags;
            //flags &= ~(NetLane.Flags.LeftForwardRight);

            //Vector3 segDir = segment.GetDirection(m_nodeId);
            //uint[] connections = GetConnectionsAsArray();
            //foreach (uint connection in connections)
            //{
            //    ushort seg = NetManager.instance.m_lanes.m_buffer[connection].m_segment;
            //    Vector3 dir = NetManager.instance.m_segments.m_buffer[seg].GetDirection(m_nodeId);
            //    if (Vector3.Angle(segDir, dir) > 150f)
            //    {
            //        flags |= NetLane.Flags.Forward;
            //    }
            //    else
            //    {

            //        if (Vector3.Dot(Vector3.Cross(segDir, -dir), Vector3.up) > 0f)
            //            flags |= NetLane.Flags.Right;
            //        else
            //            flags |= NetLane.Flags.Left;
            //    }
            //}

            //NetManager.instance.m_lanes.m_buffer[m_laneId].m_flags = (ushort)flags;
        }

        //public static uint[] GetConnectionsAsArray()
        //{
        //    uint[] connections = null;
        //    while (!Monitor.TryEnter(this.m_laneConnections, SimulationManager.SYNCHRONIZE_TIMEOUT))
        //    {
        //    }
        //    try
        //    {
        //        connections = m_laneConnections.ToArray();
        //    }
        //    finally
        //    {
        //        Monitor.Exit(this.m_laneConnections);
        //    }
        //    return connections;
        //}

        //public static int ConnectionCount()
        //{
        //    int count = 0;
        //    while (!Monitor.TryEnter(this.m_laneConnections, SimulationManager.SYNCHRONIZE_TIMEOUT))
        //    {
        //    }
        //    try
        //    {
        //        count = m_laneConnections.Count();
        //    }
        //    finally
        //    {
        //        Monitor.Exit(this.m_laneConnections);
        //    }
        //    return count;
        //}

        //public static bool ConnectsTo(uint laneId)
        //{
        //    while (!Monitor.TryEnter(this.m_laneConnections, SimulationManager.SYNCHRONIZE_TIMEOUT))
        //    {
        //    }
        //    try
        //    {
        //        if (m_laneConnections.Count == 0)
        //        {
        //            return true;
        //        }

        //        if (m_laneConnections.Contains(laneId))
        //        {
        //            NetLane lane = NetManager.instance.m_lanes.m_buffer[laneId];

        //            if ((lane.m_flags & CONTROL_BIT) != CONTROL_BIT)
        //            {
        //                m_laneConnections.Remove(laneId);
        //                return false;
        //            }
        //            else
        //            {
        //                return true;
        //            }

        //        }

        //        return false;
        //    }
        //    finally
        //    {
        //        Monitor.Exit(this.m_laneConnections);
        //    }
        //}

        //bool static FindNode(NetSegment segment)
        //{
        //    uint laneId = segment.m_lanes;
        //    NetInfo info = segment.Info;
        //    int laneCount = info.m_lanes.Length;
        //    int laneIndex = 0;
        //    for (; laneIndex < laneCount && laneId != 0; laneIndex++)
        //    {
        //        if (laneId == m_laneId)
        //            break;
        //        laneId = NetManager.instance.m_lanes.m_buffer[laneId].m_nextLane;
        //    }

        //    if (laneIndex < laneCount)
        //    {
        //        NetInfo.Direction laneDir = ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ? info.m_lanes[laneIndex].m_finalDirection : NetInfo.InvertDirection(info.m_lanes[laneIndex].m_finalDirection);

        //        if ((laneDir & (NetInfo.Direction.Forward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Forward)
        //            m_nodeId = segment.m_endNode;
        //        else if ((laneDir & (NetInfo.Direction.Backward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Backward)
        //            m_nodeId = segment.m_startNode;

        //        return true;
        //    }

        //    return false;
        //}
    }
}
