using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.ToolsV2.LaneRouting.Data;
using Transit.Addon.ToolsV2.LaneRouting.DataLegacy.TPP;

namespace Transit.Addon.ToolsV2.LaneRouting.Core
{
    public partial class RoutingDataManager : SerializableDataExtensionBase
    {
        public override void OnLoadData()
        {
            //if ((ToolModule.ActiveOptions & ToolModule.ModOptions.RoadCustomizerTool) == ToolModule.ModOptions.None)
            //    return;

            var loadedData = new List<NodeRoutingData>();

            var tppData = new TPPLaneSerializer().DeserializeData(serializableDataManager);
            if (tppData != null)
            {
                loadedData.AddRange(tppData.ConvertToNodeRouting());
            }

            var tamData = new NodeRoutingDataSerializer().DeserializeData(serializableDataManager);
            if (tamData != null)
            {
                loadedData.AddRange(loadedData);
            }

            RoutingManager.SetLoadedData(loadedData);
        }

        public override void OnSaveData()
        {
            //if ((ToolModule.ActiveOptions & ToolModule.ModOptions.RoadCustomizerTool) == ToolModule.ModOptions.None)
            //    return;

            //new NodeRoutingDataSerializer().SerializeData(serializableDataManager, _routingData.ToArray());
        }
    }
}
