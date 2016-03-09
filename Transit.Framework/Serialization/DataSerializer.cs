using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ICities;

namespace Transit.Framework.Serialization
{
    public class DataSerializer<TData, TBinder> : IDataSerializer<TData>
        where TData : class
        where TBinder : SerializationBinder, new()
    {
        public string DataId { get; private set; }

        public DataSerializer(string dataId)
        {
            DataId = dataId;
        }

        public TData DeserializeData(ISerializableData gameData)
        {
            try
            {
                var data = gameData.LoadData(DataId);
                if (data == null)
                {
                    return null;
                }

                using (var memStream = new MemoryStream())
                {
                    memStream.Write(data, 0, data.Length);
                    memStream.Position = 0;

                    var binaryFormatter = new BinaryFormatter
                    {
                        Binder = new TBinder()
                    };

                    return (TData)binaryFormatter.Deserialize(memStream);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(string.Format("TFW: Crashed-Loading Data of type " + typeof(TData).Name));
                UnityEngine.Debug.Log("TFW: " + e.Message);
                UnityEngine.Debug.Log("TFW: " + e.ToString());

                return null;
            }
        }

        public void SerializeData(ISerializableData gameData, TData data)
        {
            try
            {
                using (var memStream = new MemoryStream())
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memStream, data);
                    gameData.SaveData(DataId, memStream.ToArray());
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(string.Format("TFW: Crashed-Saving Routing configurations"));
                UnityEngine.Debug.Log("TFW: " + e.Message);
                UnityEngine.Debug.Log("TFW: " + e.ToString());
            }
        }
    }
}
