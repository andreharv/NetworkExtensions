using System;
using System.Linq;
using Transit.Addon.TM.Data;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public partial class TAMLaneRoutingManager
    {
        public bool ScrubRoute(TAMLaneRoute route)
        {
            if (route.NodeId == 0)
            {
                var foundNodeId = NetManager.instance.FindLaneNode(route.LaneId);
                if (foundNodeId == null)
                {
                    return false;
                }

                route.NodeId = foundNodeId.Value;
            }

            int startI = 0;
            while (true)
            {
                for (int i = startI; i < route.Connections.Length; ++i)
                {
                    try
                    {
                        ushort laneFlags = NetManager.instance.m_lanes.m_buffer[route.Connections[i]].m_flags;
                        if ((laneFlags & ((ushort)NetLane.Flags.Created)) == 0)
                        {
                            // lane invalid
                            route.RemoveConnection(route.Connections[i]);
                            startI = i;
                            goto CONTINUE_WHILE; // lane has been deleted; continue search for invalid lanes
                        }
                    }
                    catch (Exception e)
                    {
                        // we might get an IndexOutOfBounds here since we are not locking
#if DEBUG
                        Log.Warning("ConnectsTo: " + e.ToString());
#endif
                    }
                }
                break; // no more lanes or everything ok
                CONTINUE_WHILE:;
            }

            return true;
        }

        public void UpdateNodeArrows(ushort nodeId)
        {
            var node = NetManager.instance.m_nodes.m_buffer[nodeId];

            for (int i = 0; i < 8; i++)
            {
                var segmentId = node.GetSegment(i);
                if (segmentId == 0)
                {
                    continue;
                }

                var segment = NetManager.instance.m_segments.m_buffer[segmentId];
                var laneId = segment.m_lanes;
                var netInfo = segment.Info;
                var laneCount = netInfo.m_lanes.Length;
                for (var laneIndex = 0; laneIndex < laneCount && laneId != 0; laneIndex++)
                {
                    UpdateLaneArrows(laneId, nodeId);

                    laneId = NetManager.instance.m_lanes.m_buffer[laneId].m_nextLane;
                }
            }
        }

        public void UpdateLaneArrows(uint laneId, ushort nodeId)
        {
            if (nodeId == 0)
            {
                throw new Exception("NodeId has not been set");
            }

            if (NetManager.instance.m_nodes.m_buffer[nodeId].CountSegments() <= 2)
                return;
            
            var route = GetRoute(laneId);

            if (route == null || !route.Connections.Any())
            {
                UpdateLaneDefaultArrows(laneId);
            }
            else
            {
                UpdateLaneRoutedArrows(route);
            }
        }

        private void UpdateLaneDefaultArrows(uint laneId)
        {
            var lane = NetManager.instance.m_lanes.m_buffer[laneId];
            var segmentId = lane.m_segment;
            var segment = NetManager.instance.m_segments.m_buffer[lane.m_segment];
            var netInfo = segment.Info;
            netInfo.m_netAI.UpdateLanes(segmentId, ref segment, false);
        }

        private void UpdateLaneRoutedArrows(TAMLaneRoute route)
        {
            var lane = NetManager.instance.m_lanes.m_buffer[route.LaneId];
            var flags = (NetLane.Flags)lane.m_flags;
            flags &= ~(NetLane.Flags.LeftForwardRight);

            //var segment = NetManager.instance.m_segments.m_buffer[lane.m_segment];
            //var segmentDir = segment.GetDirection(route.NodeId);
            foreach (var connLaneId in route.Connections)
            {
                flags |= (NetLane.Flags) (int)GetRouteDirection(route.LaneId, connLaneId, route.NodeId);

                //var connSegmentId = NetManager.instance.m_lanes.m_buffer[connLaneId].m_segment;
                //var connSegmentDir = NetManager.instance.m_segments.m_buffer[connSegmentId].GetDirection(route.NodeId);
                //if (Vector3.Angle(segmentDir, connSegmentDir) > 150f)
                //{
                //    flags |= NetLane.Flags.Forward;
                //}
                //else
                //{
                //    if (Vector3.Dot(Vector3.Cross(segmentDir, -connSegmentDir), Vector3.up) > 0f)
                //        flags |= NetLane.Flags.Right;
                //    else
                //        flags |= NetLane.Flags.Left;
                //}
            }

            lane.m_flags = (ushort)flags;
        }
    }
}
