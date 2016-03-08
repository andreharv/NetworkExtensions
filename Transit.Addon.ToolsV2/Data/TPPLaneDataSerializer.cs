using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CSL_Traffic;
using ICities;
using UnityEngine;

namespace Transit.Addon.ToolsV2.Data
{
    public class TPPLaneDataSerializer : SerializableDataExtensionBase
    {
        private const string LANE_DATA_ID = "Traffic++_RoadManager_Lanes";
            
        public override void OnLoadData()
        {
            if ((ToolModuleV2.ActiveOptions & ModOptions.RoadCustomizerTool) == ModOptions.None)
                return;

            try
            {
                Logger.LogInfo("Loading road data. Time: " + Time.realtimeSinceStartup);
                byte[] data = serializableDataManager.LoadData(LANE_DATA_ID);
                if (data == null)
                {
                    Logger.LogInfo("No road data to load.");
                    return;
                }

                using (var memStream = new MemoryStream())
                {
                    memStream.Write(data, 0, data.Length);
                    memStream.Position = 0;

                    var binaryFormatter = new BinaryFormatter()
                    {
                        Binder = new TPPLaneDataSerializationBinder()
                    };
                    TPPLaneDataManager.sm_lanes = (TPPLaneData[]) binaryFormatter.Deserialize(memStream);
                }

                foreach (TPPLaneData lane in TPPLaneDataManager.sm_lanes)
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

                Logger.LogInfo("Finished loading road data. Time: " + Time.realtimeSinceStartup);
            }
            catch (Exception e)
            {
                Logger.LogError("Unexpected " + e.GetType().Name + " loading road data.");
            }
            finally
            {
                if (TPPLaneDataManager.sm_lanes == null)
                {
                    TPPLaneDataManager.sm_lanes = new TPPLaneData[NetManager.MAX_LANE_COUNT];
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
                serializableDataManager.SaveData(LANE_DATA_ID, memStream.ToArray());
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
