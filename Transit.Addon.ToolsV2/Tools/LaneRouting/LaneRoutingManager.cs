using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ColossalFramework;
using Transit.Addon.ToolsV2.LaneRouting.Data;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures;
using UnityEngine;

namespace Transit.Addon.ToolsV2.LaneRouting
{
    public class LaneRoutingManager : Singleton<LaneRoutingManager>, IExtendedLaneRoutingManager
    {
        private static IDictionary<uint, NodeRoutingData> _routingData;

        public static void SetLoadedData(IEnumerable<NodeRoutingData> loadedData)
        {
            _routingData = new Dictionary<uint, NodeRoutingData>();
            foreach (var data in loadedData.Where(d => d.IsRelevant()))
            {
                _routingData[data.NodeId] = data;
            }
        }

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
                var newData = new NodeRoutingData {NodeId = nodeId};
                Monitor.Enter(_routingData);
                try
                {
                    _routingData[nodeId] = newData;
                }
                finally
                {
                    Monitor.Exit(_routingData);
                }
            }

            return _routingData[nodeId];
        }

        public bool CanLanesConnect(ushort nodeId, uint laneId1, uint laneId2)
        {
            if (_routingData == null)
            {
                throw new Exception("Routing has not been initialized/loaded yet");
            }

            if (!_routingData.ContainsKey(nodeId))
            {
                return true;
            }


            NodeRoutingData nodeRouting;
            Monitor.Enter(_routingData);
            try
            {
                nodeRouting = _routingData[nodeId];
            }
            finally
            {
                Monitor.Exit(_routingData);
            }


            return nodeRouting.HasRouteFor(laneId1, laneId2);
        }

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
