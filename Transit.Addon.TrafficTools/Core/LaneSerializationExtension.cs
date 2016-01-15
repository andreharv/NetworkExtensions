using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ICities;
using Transit.Addon.TrafficTools.Tools;
using UnityEngine;

namespace Transit.Addon.TrafficTools.Core
{
    public class LaneSerializationExtension : SerializableDataExtensionBase
    {
        const string LANES_DATA_ID = "Traffic++_RoadManager_Lanes";
            
        public override void OnLoadData()
        {
            if ((TrafficToolModule.ActiveOptions & TrafficToolModule.ModOptions.RoadCustomizerTool) == TrafficToolModule.ModOptions.None || (TrafficToolModule.ActiveOptions & TrafficToolModule.ModOptions.GhostMode) == TrafficToolModule.ModOptions.GhostMode)
                return;
                

            Logger.LogInfo("Loading road data. Time: " + Time.realtimeSinceStartup);
            byte[] data = serializableDataManager.LoadData(LANES_DATA_ID);
            if (data == null)
            {
                Logger.LogInfo("No road data to load.");
                return;
            }

            MemoryStream memStream = new MemoryStream();
            memStream.Write(data, 0, data.Length);
            memStream.Position = 0;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Binder = new LaneSerializationBinder();
            try
            {
                LanesManager.sm_lanes = (Lane[]) binaryFormatter.Deserialize(memStream);
                    
                FastList<ushort> nodesList = new FastList<ushort>();
                foreach (Lane lane in LanesManager.sm_lanes)
                {
                    if (lane == null)
                        continue;

                    lane.UpdateArrows();
                    if (lane.ConnectionCount() > 0)
                        nodesList.Add(lane.m_nodeId);

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

                RoadCustomizerTool customizerTool = ToolsModifierControl.GetTool<RoadCustomizerTool>();
                foreach (ushort nodeId in nodesList)
                    customizerTool.SetNodeMarkers(nodeId);

                Logger.LogInfo("Finished loading road data. Time: " + Time.realtimeSinceStartup);
            }
            catch (Exception e)
            {
                Logger.LogError("Unexpected " + e.GetType().Name + " loading road data.");
            }
            finally
            {
                memStream.Close();
            }
        }

        public override void OnSaveData()
        {
            if ((TrafficToolModule.ActiveOptions & TrafficToolModule.ModOptions.RoadCustomizerTool) == TrafficToolModule.ModOptions.None || (TrafficToolModule.ActiveOptions & TrafficToolModule.ModOptions.GhostMode) == TrafficToolModule.ModOptions.GhostMode)
                return;

            Logger.LogInfo("Saving road data!");
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            try
            {
                binaryFormatter.Serialize(memStream, LanesManager.sm_lanes);
                serializableDataManager.SaveData(LANES_DATA_ID, memStream.ToArray());
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
