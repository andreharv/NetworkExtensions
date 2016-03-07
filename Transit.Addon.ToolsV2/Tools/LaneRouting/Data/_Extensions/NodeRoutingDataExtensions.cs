using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Transit.Addon.ToolsV2.LaneRouting.Data
{
    public static partial class NodeRoutingDataExtensions
    {
        public static bool IsRelevant(this NodeRoutingData data)
        {
            if (!data.HasRoutes)
            {
                return false;
            }

            var node = NetManager.instance.GetNode(data.NodeId);
            if (node == null)
            {
                return false;
            }

            return node.Value.CountSegments() > 1;
        }

        public static void AddRoute(this NodeRoutingData nodeRouting, LaneRoutingData route)
        {
            if (!nodeRouting.Routes.Contains(route))
            {
                NetManager.instance.AddLaneFlag(LaneRoutingData.LANEROUTING_CONTROL_BIT, route.OriginLaneId);
                Monitor.Enter(nodeRouting.Routes);
                try
                {
                    nodeRouting.Routes.Add(route);
                }
                finally
                {
                    Monitor.Exit(nodeRouting.Routes);
                }
            }

            nodeRouting.UpdateArrows();
        }

        public static void RemoveRoute(this NodeRoutingData nodeRouting, LaneRoutingData route)
        {
            if (nodeRouting.Routes.Contains(route))
            {
                Monitor.Enter(nodeRouting.Routes);
                try
                {
                    nodeRouting.Routes.Remove(route);
                }
                finally
                {
                    Monitor.Exit(nodeRouting.Routes);
                }

                if (nodeRouting.Routes.Count == 0)
                {
                    NetManager.instance.RemoveLaneFlag(LaneRoutingData.LANEROUTING_CONTROL_BIT, route.OriginLaneId);
                }
            }

            nodeRouting.UpdateArrows();
        }

        public static void VerifyRoutes(this NodeRoutingData nodeRouting)
        {
            ICollection<LaneRoutingData> obsoleteRoutes = null;

            foreach (var route in nodeRouting.Routes)
            {
                var originLane = NetManager.instance.GetLane(LaneRoutingData.LANEROUTING_CONTROL_BIT, route.OriginLaneId);
                if (originLane == null)
                {
                    if (obsoleteRoutes == null)
                    {
                        obsoleteRoutes = new HashSet<LaneRoutingData>();
                    }
                    obsoleteRoutes.Add(route);
                    continue;
                }

                var destinationLane = NetManager.instance.GetLane(LaneRoutingData.LANEROUTING_CONTROL_BIT, route.DestinationLaneId);
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
                Monitor.Enter(nodeRouting.Routes);
                try
                {
                    foreach (var route in obsoleteRoutes)
                    {
                        nodeRouting.Routes.Remove(route);
                    }
                }
                finally
                {
                    Monitor.Exit(nodeRouting.Routes);
                }
            }
        }

        public static bool HasRouteFor(this NodeRoutingData nodeRouting, uint originLaneId, uint destinationLaneId)
        {
            LaneRoutingData[] laneRoutes;
            Monitor.Enter(nodeRouting.Routes);
            try
            {
                laneRoutes = nodeRouting
                    .Routes
                    .Where(r => r.OriginLaneId == originLaneId)
                    .ToArray();
            }
            finally
            {
                Monitor.Exit(nodeRouting.Routes);
            }


            if (!laneRoutes.Any())
            {
                return true;
            }
            else
            {
                return laneRoutes.Any(r => r.DestinationLaneId == destinationLaneId);
            }
        }
    }
}
