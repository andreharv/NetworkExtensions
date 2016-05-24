using System.Collections.Generic;
using System.Linq;
using Transit.Addon.TM.Data;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public partial class TAMLaneRoutingManager
    {
        private TAMLaneRoute TransformToRoute(uint laneId, TAMLaneDirection directions)
        {
            if (directions == TAMLaneDirection.None)
            {
                return null;
            }

            var nodeId = NetManager.instance.FindLaneNodeId(laneId);
            if (nodeId == null)
            {
                return null;
            }

            var connections = NetManager.instance.GetConnectingLanes(laneId, (NetLane.Flags)directions).ToArray();
            if (!connections.Any())
            {
                return null;
            }

            return new TAMLaneRoute()
            {
                LaneId = laneId,
                NodeId = nodeId.Value,
                Connections = connections.ToArray()
            };
        }

        //public void RemoveLaneDirection(uint laneId)
        //{
        //    if (laneId <= 0)
        //        return;

        //    _laneDirections[laneId] = null;
        //    uint laneFlags = (uint)Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags;

        //    if (((NetLane.Flags)laneFlags & NetLane.Flags.Created) == NetLane.Flags.None)
        //    {
        //        Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags = 0;
        //    }
        //    else
        //    {
        //        Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags &= (ushort)~Flags.lfr;
        //    }
        //}

        public void ScrubSegment(uint segmentId)
        {
            //uint laneId = NetManager.instance.m_segments.m_buffer[segmentId].m_lanes;
            //while (laneId != 0)
            //{
            //    if (!ApplyLaneDirection(laneId))
            //    {
            //        RemoveLaneDirection(laneId);
            //    }
            //    laneId = NetManager.instance.m_lanes.m_buffer[laneId].m_nextLane;
            //}
        }
    }
}
