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
        private const string LANE_DATAV2_ROUTES_ID = "Traffic++V2_Routes";
        private const string LANE_DATAV2_RESTRICTIONS_ID = "Traffic++V2_Restrictions";
        private const string LANE_DATAV2_SPEEDLIMIT_ID = "Traffic++V2_SpeedLimit";

        public override void OnLoadData()
        {
            var dataV1 = new DataSerializer<TPPLaneDataV1[], TPPDataSerializationBinder>().DeserializeData(serializableDataManager, LANE_DATAV1_ID);
            var routeDataV2 = new DataSerializer<TAMLaneRoute[], TPPDataSerializationBinder>().DeserializeData(serializableDataManager, LANE_DATAV2_ROUTES_ID);
            var restrictionDataV2 = new DataSerializer<TAMLaneRestriction[], TPPDataSerializationBinder>().DeserializeData(serializableDataManager, LANE_DATAV2_RESTRICTIONS_ID);
            var speedLimitDataV2 = new DataSerializer<TAMLaneSpeedLimit[], TPPDataSerializationBinder>().DeserializeData(serializableDataManager, LANE_DATAV2_SPEEDLIMIT_ID);

            if (routeDataV2 != null)
            {
                Log.Info("Using T++ V2 data");
            }
            else
            {
                if (dataV1 != null)
                {
                    Log.Info("Using T++ V1 data");
                    routeDataV2 = dataV1
                        .Select(d => d == null ? null : d.ConvertToRoute())
                        .ToArray();
                    restrictionDataV2 = dataV1
                        .Select(d => d == null ? null : d.ConvertToRestriction())
                        .ToArray();
                    speedLimitDataV2 = dataV1
                        .Select(d => d == null ? null : d.ConvertToSpeedLimit())
                        .ToArray();
                }
            }

            TPPLaneRoutingManager.instance.Init(routeDataV2);
            TPPRoadRestrictionManager.instance.Init(restrictionDataV2);
            TPPLaneSpeedManager.instance.Init(speedLimitDataV2);
        }

        public override void OnSaveData()
        {
            if ((ToolModuleV2.ActiveOptions & Options.RoadCustomizerTool) == Options.None)
                return;

            Log.Info("Saving T++ road data!");

            if (TPPLaneRoutingManager.instance.IsLoaded())
            {
                new DataSerializer<TAMLaneRoute[], TPPDataSerializationBinder>().SerializeData(serializableDataManager, LANE_DATAV2_ROUTES_ID, TPPLaneRoutingManager.instance.GetAllRoutes());
            }
            if (TPPRoadRestrictionManager.instance.IsLoaded())
            {
                new DataSerializer<TAMLaneRestriction[], TPPDataSerializationBinder>().SerializeData(serializableDataManager, LANE_DATAV2_RESTRICTIONS_ID, TPPRoadRestrictionManager.instance.GetAllLaneRestrictions());
            }
            if (TPPLaneSpeedManager.instance.IsLoaded())
            {
                new DataSerializer<TAMLaneSpeedLimit[], TPPDataSerializationBinder>().SerializeData(serializableDataManager, LANE_DATAV2_SPEEDLIMIT_ID, TPPLaneSpeedManager.instance.GetAllLaneData());
            }
        }
    }
}
