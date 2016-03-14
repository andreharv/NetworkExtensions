using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ICities;
using Transit.Addon.ToolsV2.Data;
using Transit.Addon.ToolsV2.PathFindingFeatures;
using Transit.Framework.Serialization;

namespace Transit.Addon.ToolsV2.DataSerialization
{
    public class TPPLaneDataSerializer : SerializableDataExtensionBase
    {
        private const string LANE_DATAV1_ID = "Traffic++_RoadManager_Lanes";
        private const string LANE_DATAV2_ID = "Traffic++V2_RoadManager_Lanes";
            
        public override void OnLoadData()
        {
            var dataV1 = new DataSerializer<TPPLaneDataV1[], TPPLaneDataSerializationBinder>(LANE_DATAV1_ID).DeserializeData(serializableDataManager);
            var dataV2 = new DataSerializer<TPPLaneDataV2[], TPPLaneDataSerializationBinder>(LANE_DATAV2_ID).DeserializeData(serializableDataManager);

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
            if ((ToolModuleV2.ActiveOptions & ModOptions.RoadCustomizerTool) == ModOptions.None)
                return;

            Logger.LogInfo("Saving road data!");
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            try
            {
                binaryFormatter.Serialize(memStream, TPPLaneDataManager.sm_lanes);
                serializableDataManager.SaveData(LANE_DATAV2_ID, memStream.ToArray());
                Logger.LogInfo("Finished saving road data!");
            }
            catch (Exception e)
            {
                Logger.LogError("Unexpected " + e.GetType().Name + " saving road data.");
            }
            finally
            {
                memStream.Close();
            }
        }
    }
}
