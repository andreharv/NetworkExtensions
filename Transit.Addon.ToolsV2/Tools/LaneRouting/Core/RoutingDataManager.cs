using ICities;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.ToolsV2.LaneRouting.Data;
using Transit.Addon.ToolsV2.LaneRouting.DataLegacy.TPP;
using Transit.Addon.ToolsV2.LaneRouting.Markers;
using UnityEngine;

namespace Transit.Addon.ToolsV2.LaneRouting.Core
{
    public class RoutingDataManager : SerializableDataExtensionBase
    {
        private static ICollection<NodeRoutingData> _routingData;

        public override void OnLoadData()
        {
            //if ((ToolModule.ActiveOptions & ToolModule.ModOptions.RoadCustomizerTool) == ToolModule.ModOptions.None)
            //    return;

            var routingData = new List<NodeRoutingData>();

            var tppData = new TPPLaneSerializer().DeserializeData(serializableDataManager);
            if (tppData != null)
            {
                routingData.AddRange(tppData.ConvertToNodeRouting());
            }

            var loadedData = new NodeRoutingDataSerializer().DeserializeData(serializableDataManager);
            if (loadedData != null)
            {
                routingData.AddRange(loadedData);
            }

            _routingData = routingData
                .Where(NodeRoutingMarker.IsDataRelevant)
                .ToList();

            // TODO: Make sure to initialize the tool Waaayy before here
            var routingTool = ToolsModifierControl.GetTool<RoutingTool>();
            routingTool.CreateInitialMarkers(_routingData);

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
