using System;
using System.Collections.Generic;
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
            if (route.LaneId == 0)
            {
                //Log.Info("LaneRoute has been removed, LaneId is 0");
                return false;
            }

            if (route.NodeId == 0)
            {
                var foundNodeId = NetManager.instance.FindLaneNodeId(route.LaneId);
                if (foundNodeId == null || foundNodeId.Value == 0)
                {
                    //Log.Info(string.Format("LaneRoute {0} has been removed, no NodeId has been found", route.LaneId));
                    return false;
                }
                
                route.NodeId = foundNodeId.Value;
            }

            var lane = NetManager.instance.m_lanes.m_buffer[route.LaneId];
            if (((NetLane.Flags)lane.m_flags & NetLane.Flags.Created) == NetLane.Flags.None)
            {
                //Log.Info(string.Format("LaneRoute {0} has been removed, lane has not been created", route.LaneId));
                return false;
            }

            ICollection<uint> routes;
            while (true)
            {
                try
                {
                    routes = route.Connections.ToArray();
                    break;
                }
                catch (Exception e)
                {
                    // we might get an IndexOutOfBounds here since we are not locking
#if DEBUG
                        Log.Warning("ConnectsTo: " + e.ToString());
#endif
                }
            }

            var remainingRoutes = new List<uint>();
            foreach (var destinationLaneId in routes.Distinct())
            {
                if (destinationLaneId == 0)
                {
                    continue;
                }

                var destinationLane = NetManager.instance.m_lanes.m_buffer[destinationLaneId];
                if (((NetLane.Flags)destinationLane.m_flags & NetLane.Flags.Created) == NetLane.Flags.None)
                {
                    continue;
                }

                remainingRoutes.Add(destinationLaneId);
            }

            if (remainingRoutes.Count == 0)
            {
                //Log.Info(string.Format("LaneRoute {0} has been removed, no connection has been found", route.LaneId));
                return false;
            }
            
            while (true)
            {
                try
                {
                    route.Connections = remainingRoutes.ToArray();
                    break;
                }
                catch (Exception e)
                {
                    // we might get an IndexOutOfBounds here since we are not locking
#if DEBUG
                        Log.Warning("ConnectsTo: " + e.ToString());
#endif
                }
            }

            Log.Info(string.Format("LaneRoute {0} is valid!", route.LaneId));
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
            Log.Info(string.Format("UpdateLaneArrows NodeId:{0} LaneId:{1}", nodeId, laneId));

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
            Log.Info("UpdateLaneDefaultArrows");
            var lane = NetManager.instance.m_lanes.m_buffer[laneId];
            var segmentId = lane.m_segment;
            var segment = NetManager.instance.m_segments.m_buffer[lane.m_segment];
            var netInfo = segment.Info;
            netInfo.m_netAI.UpdateLanes(segmentId, ref segment, false);
        }

        private void UpdateLaneRoutedArrows(TAMLaneRoute route)
        {
            var directions = NetLane.Flags.None;

            foreach (var connLaneId in route.Connections)
            {
                directions |= (NetLane.Flags) (int)GetRouteDirection(route.LaneId, connLaneId, route.NodeId);
            }
            
            Log.Info(string.Format("UpdateLaneRoutedArrows to {0}", directions));
            
            var flags = (NetLane.Flags)NetManager.instance.m_lanes.m_buffer[route.LaneId].m_flags;
            flags &= ~NetLane.Flags.LeftForwardRight;
            flags |= directions;
            NetManager.instance.m_lanes.m_buffer[route.LaneId].m_flags = (ushort)flags;
        }
    }
}
