using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ICities;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.Data.Legacy;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Framework.Serialization;
using Transit.Framework;

namespace Transit.Addon.TM.DataSerialization
{
    public class TPPLaneDataSerializer : SerializableDataExtensionBase
    {
        private const string LANE_DATAV1_ID = "Traffic++_RoadManager_Lanes";
        private const string LANE_DATAV2_ID = "Traffic++V2_RoadManager_Lanes";
            
        public override void OnLoadData()
        {
            var dataV1 = new DataSerializer<TPPLaneDataV1[], TPPLaneDataSerializationBinder>().DeserializeData(serializableDataManager, LANE_DATAV1_ID);
            var dataV2 = new DataSerializer<TPPLaneDataV2[], TPPLaneDataSerializationBinder>().DeserializeData(serializableDataManager, LANE_DATAV2_ID);

            TPPLaneDataManager.sm_lanes = null;

            if (dataV2 != null)
            {
                TPPLaneDataManager.sm_lanes = dataV2;
            }
            else
            {
                if (dataV1 != null)
                {
                    TPPLaneDataManager.sm_lanes = dataV1
                        .Select(d => d == null ? null : d.ConvertToV2())
                        .ToArray();
                }
            }

            if (TPPLaneDataManager.sm_lanes == null)
            {
                TPPLaneDataManager.sm_lanes = new TPPLaneDataV2[NetManager.MAX_LANE_COUNT];
            }

            foreach (TPPLaneDataV2 lane in TPPLaneDataManager.sm_lanes)
            {
                if (lane == null)
                    continue;

                lane.UpdateArrows();

                if (lane.m_speed == 0)
                {
                    NetSegment segment = NetManager.instance.m_segments.m_buffer[NetManager.instance.m_lanes.m_buffer[lane.m_laneId].m_segment];
                    NetInfo info = segment.Info;
                    uint l = segment.m_lanes;
                    int n = 0;
                    while (l != lane.m_laneId && n < info.m_lanes.Length)
                    {
                        l = NetManager.instance.m_lanes.m_buffer[l].m_nextLane;
                        n++;
                    }

                    if (n < info.m_lanes.Length)
                        lane.m_speed = info.m_lanes[n].m_speedLimit;
                }

            }
        }

        public override void OnSaveData()
        {
            if ((ToolModuleV2.ActiveOptions & Options.RoadCustomizerTool) == Options.None)
                return;

            Log.Info("Saving road data!");

            if (TPPLaneDataManager.sm_lanes != null)
            {
                new DataSerializer<TPPLaneDataV2[], TPPLaneDataSerializationBinder>().SerializeData(serializableDataManager, LANE_DATAV2_ID, TPPLaneDataManager.sm_lanes);
            }
        }
    }
}
