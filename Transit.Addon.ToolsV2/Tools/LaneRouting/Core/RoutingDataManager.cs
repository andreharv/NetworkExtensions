using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.ToolsV2.LaneRouting.Data;
using Transit.Addon.ToolsV2.LaneRouting.DataLegacy.TPP;

namespace Transit.Addon.ToolsV2.LaneRouting.Core
{
    public class RoutingDataManager : SerializableDataExtensionBase
    {
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

        public override void OnLoadData()
        {
            //if ((ToolModule.ActiveOptions & ToolModule.ModOptions.RoadCustomizerTool) == ToolModule.ModOptions.None)
            //    return;

            var loadedData = new List<NodeRoutingData>();

            var tppData = new TPPLaneSerializer().DeserializeData(serializableDataManager);
            if (tppData != null)
            {
                loadedData.AddRange(tppData.ConvertToNodeRouting());
            }

            var tamData = new NodeRoutingDataSerializer().DeserializeData(serializableDataManager);
            if (tamData != null)
            {
                loadedData.AddRange(loadedData);
            }

            _routingData = new Dictionary<uint, NodeRoutingData>();
            foreach (var data in loadedData.Where(d => d.IsRelevant()))
            {
                _routingData[data.NodeId] = data;
            }

            //// TODO: Make sure to initialize the tool Waaayy before here
            //var routingTool = ToolsModifierControl.GetTool<RoutingTool>();
            //routingTool.CreateInitialMarkers(_routingData);

            //FastList<ushort> nodesList = new FastList<ushort>();
            //foreach (TPPLaneData lane in lanes)
            //{
            //    if (lane == null)
            //        continue;

            //    lane.UpdateArrows();
            //    if (lane.ConnectionCount() > 0)
            //        nodesList.Add(lane.m_nodeId);

            //    //if (lane.m_speed == 0)
            //    //{
            //    //    NetSegment segment = NetManager.instance.m_segments.m_buffer[NetManager.instance.m_lanes.m_buffer[lane.m_laneId].m_segment];
            //    //    NetInfo info = segment.Info;
            //    //    uint l = segment.m_lanes;
            //    //    int n = 0;
            //    //    while (l != lane.m_laneId && n < info.m_lanes.Length)
            //    //    {
            //    //        l = NetManager.instance.m_lanes.m_buffer[l].m_nextLane;
            //    //        n++;
            //    //    }

            //    //    if (n < info.m_lanes.Length)
            //    //        lane.m_speed = info.m_lanes[n].m_speedLimit;
            //    //}
        }

        public override void OnSaveData()
        {
            //if ((ToolModule.ActiveOptions & ToolModule.ModOptions.RoadCustomizerTool) == ToolModule.ModOptions.None)
            //    return;

            //new NodeRoutingDataSerializer().SerializeData(serializableDataManager, _routingData.ToArray());
        }
    }
}
