using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ICities;
using Transit.Addon.ToolsV2.Common;
using UnityEngine;

namespace Transit.Addon.ToolsV2.LaneRouting.DataLegacy.TPP
{
    public class TPPLaneSerializer : ISerializer<TPPLaneData[]>
    {
        private const string DATA_ID = "Traffic++_RoadManager_Lanes";

        public TPPLaneData[] DeserializeData(ISerializableData gameData)
        {
            byte[] data = gameData.LoadData(DATA_ID);
            if (data == null)
            {
                return null;
            }

            try
            {
                using (var memStream = new MemoryStream())
                {
                    memStream.Write(data, 0, data.Length);
                    memStream.Position = 0;

                    var binaryFormatter = new BinaryFormatter
                    {
                        Binder = new TPPLaneDataSerializationBinder()
                    };

                    return (TPPLaneData[])binaryFormatter.Deserialize(memStream);
                }
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("TAMTool: Crashed-Loading Lane configurations"));
                Debug.Log("TAMTool: " + e.Message);
                Debug.Log("TAMTool: " + e.ToString());

                return null;
            }
        }

        public void SerializeData(ISerializableData gameData, TPPLaneData[] data)
        {
            try
            {
                using (var memStream = new MemoryStream())
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memStream, data);
                    gameData.SaveData(DATA_ID, memStream.ToArray());
                }
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("TAMTool: Crashed-Saving Lane configurations"));
                Debug.Log("TAMTool: " + e.Message);
                Debug.Log("TAMTool: " + e.ToString());
            }
        }
    }
}
