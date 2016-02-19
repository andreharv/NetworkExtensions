using ICities;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Transit.Framework.Light;
using UnityEngine;

namespace CSL_Traffic
{
    public static partial class RoadManager
    {
        public class Data : SerializableDataExtensionBase
        {
            const string LANE_DATA_ID = "Traffic++_RoadManager_Lanes";
            
            public override void OnLoadData()
            {
                if ((TrafficMod.Options & OptionsManager.ModOptions.BetaTestRoadCustomizerTool) == OptionsManager.ModOptions.None || (TrafficMod.Options & OptionsManager.ModOptions.GhostMode) == OptionsManager.ModOptions.GhostMode)
                    return;
                

                Logger.LogInfo("Loading road data. Time: " + Time.realtimeSinceStartup);
                byte[] data = serializableDataManager.LoadData(LANE_DATA_ID);
                if (data == null)
                {
                    Logger.LogInfo("No road data to load.");
                    return;
                }

                MemoryStream memStream = new MemoryStream();
                memStream.Write(data, 0, data.Length);
                memStream.Position = 0;

                BinaryFormatter binaryFormatter = new BinaryFormatter()
                {
                    Binder = new LaneSerializationBinder()
                };
                try
                {
                    RoadManager.sm_lanes = (Lane[]) binaryFormatter.Deserialize(memStream);
                    
                    FastList<ushort> nodesList = new FastList<ushort>();
                    foreach (Lane lane in RoadManager.sm_lanes)
                    {
                        if (lane == null)
                            continue;

                        if ((TrafficMod.Options & OptionsManager.ModOptions.FixCargoTrucksNotSpawning) == OptionsManager.ModOptions.FixCargoTrucksNotSpawning && lane.m_vehicleTypes == (VehicleType.ServiceVehicles | VehicleType.PassengerCar))
                            lane.m_vehicleTypes = VehicleType.All;

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
                if ((TrafficMod.Options & OptionsManager.ModOptions.BetaTestRoadCustomizerTool) == OptionsManager.ModOptions.None || 
                    (TrafficMod.Options & OptionsManager.ModOptions.GhostMode) == OptionsManager.ModOptions.GhostMode)
                    return;

                Logger.LogInfo("Saving road data!");
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MemoryStream memStream = new MemoryStream();
                try
                {
                    binaryFormatter.Serialize(memStream, RoadManager.sm_lanes);
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
}
