using System.Collections.Generic;
using System.Linq;
using ICities;
using Transit.Addon.ToolsV2.LaneRouting.Data;
using Transit.Addon.ToolsV2.LaneRouting.DataLegacy.TPP;

namespace Transit.Addon.ToolsV2.LaneRouting
{
    public class LaneRoutingSerializationHandler : SerializableDataExtensionBase
    {
        public override void OnLoadData()
        {
            var loadedData = new List<NodeRoutingData>();

            var tppData = new TPPLaneSerializer().DeserializeData(serializableDataManager);
            if (tppData != null)
            {
                loadedData.AddRange(tppData.ConvertToNodeRouting());
            }
            
            var tamData = new NodeRoutingDataSerializer().DeserializeData(serializableDataManager);
            if (tamData != null)
            {
                loadedData.AddRange(tamData);
            }

            LaneRoutingManager.SetLoadedData(loadedData);
        }

        public override void OnSaveData()
        {
            var savedData = LaneRoutingManager
                .GetAllData()
                .Where(r => r.Routes.Count > 0)
                .ToArray();

            new NodeRoutingDataSerializer().SerializeData(serializableDataManager, savedData);
        }
    }
}
