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
    public class TPPDataSerializer : SerializableDataExtensionBase
    {
        private const string LANE_DATAV1_ID = "Traffic++_RoadManager_Lanes";
        private const string LANE_DATAV2_ID = "Traffic++V2_RoadManager_Lanes";
            
        public override void OnLoadData()
        {
            var dataV1 = new DataSerializer<TPPLaneDataV1[], TPPDataSerializationBinder>().DeserializeData(serializableDataManager, LANE_DATAV1_ID);
            var dataV2 = new DataSerializer<TPPLaneDataV2[], TPPDataSerializationBinder>().DeserializeData(serializableDataManager, LANE_DATAV2_ID);
            
            if (dataV2 != null)
            {
                Log.Info("Using T++ V2 data");
            }
            else
            {
                if (dataV1 != null)
                {
                    Log.Info("Using T++ V1 data");
                    dataV2 = dataV1
                        .Select(d => d == null ? null : d.ConvertToV2())
                        .ToArray();
                }
            }

            TPPDataManager.instance.Init(dataV2);
        }

        public override void OnSaveData()
        {
            if ((ToolModuleV2.ActiveOptions & Options.RoadCustomizerTool) == Options.None)
                return;

            Log.Info("Saving T++ road data!");

            if (TPPDataManager.instance.IsLoaded())
            {
                new DataSerializer<TPPLaneDataV2[], TPPDataSerializationBinder>().SerializeData(serializableDataManager, LANE_DATAV2_ID, TPPDataManager.instance.GetAllLanes());
            }
        }
    }
}
