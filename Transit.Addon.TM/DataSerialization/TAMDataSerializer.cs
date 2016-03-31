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
    public partial class TAMDataSerializer : SerializableDataExtensionBase
    {
        private const string TPPLEGACY_LANEDATA_ID = "Traffic++_RoadManager_Lanes";
        private const string TAM_ROUTEDATA_ID = "TAM_Routes_V1.0";
        private const string TAM_RESTRICTIONDATA_ID = "TAM_Restrictions_V1.0";
        private const string TAM_SPEEDLIMITDATA_ID = "TAM_SpeedLimits_V1.0";

        public override void OnLoadData()
        {
            LoadTAMData();
            LoadTMData();
        }

        private void LoadTAMData()
        {
            var routeData = new DataSerializer<TAMLaneRoute[], TAMDataSerializationBinder>().DeserializeData(serializableDataManager, TAM_ROUTEDATA_ID);
            var restrictionData = new DataSerializer<TAMLaneRestriction[], TAMDataSerializationBinder>().DeserializeData(serializableDataManager, TAM_RESTRICTIONDATA_ID);
            var speedLimitData = new DataSerializer<TAMLaneSpeedLimit[], TAMDataSerializationBinder>().DeserializeData(serializableDataManager, TAM_SPEEDLIMITDATA_ID);

            if (routeData != null || 
                restrictionData != null || 
                speedLimitData != null)
            {
                Log.Info("Using TAM data");
            }
            else
            {
                var tppLegacyData = new DataSerializer<TPPLaneDataV1[], TPPDataSerializationBinder>().DeserializeData(serializableDataManager, TPPLEGACY_LANEDATA_ID);
                if (tppLegacyData != null)
                {
                    Log.Info("Using T++ data");
                    routeData = tppLegacyData
                        .Select(d => d == null ? null : d.ConvertToRoute())
                        .ToArray();
                    restrictionData = tppLegacyData
                        .Select(d => d == null ? null : d.ConvertToRestriction())
                        .ToArray();
                    speedLimitData = tppLegacyData
                        .Select(d => d == null ? null : d.ConvertToSpeedLimit())
                        .ToArray();
                }
            }

            try
            {
                TAMRestrictionManager.instance.Load(restrictionData);
            }
            catch (Exception ex)
            {
                Log.Error("TAM: Crashed-Loading TAMRestrictionManager");
                Log.Error("TAM: " + ex.Message);
                Log.Error("TAM: " + ex.ToString());
            }

            try
            {
                TAMSpeedLimitManager.instance.Load(speedLimitData);
            }
            catch (Exception ex)
            {
                Log.Error("TAM: Crashed-Loading TAMSpeedLimitManager");
                Log.Error("TAM: " + ex.Message);
                Log.Error("TAM: " + ex.ToString());
            }

            try
            {
                TAMLaneRoutingManager.instance.Load(routeData);
            }
            catch (Exception ex)
            {
                Log.Error("TAM: Crashed-Loading TAMLaneRoutingManager");
                Log.Error("TAM: " + ex.Message);
                Log.Error("TAM: " + ex.ToString());
            }
        }

        public override void OnSaveData()
        {
            SaveTAMData();
            SaveTMData();
        }

        private void SaveTAMData()
        {
            Log.Info("Saving TAM data!");

            if (TAMLaneRoutingManager.instance.IsLoaded())
            {
                new DataSerializer<TAMLaneRoute[], TPPDataSerializationBinder>().SerializeData(serializableDataManager, TAM_ROUTEDATA_ID, TAMLaneRoutingManager.instance.GetAllRoutes());
            }
            if (TAMRestrictionManager.instance.IsLoaded())
            {
                new DataSerializer<TAMLaneRestriction[], TPPDataSerializationBinder>().SerializeData(serializableDataManager, TAM_RESTRICTIONDATA_ID, TAMRestrictionManager.instance.GetAllLaneRestrictions());
            }
            if (TAMSpeedLimitManager.instance.IsLoaded())
            {
                new DataSerializer<TAMLaneSpeedLimit[], TPPDataSerializationBinder>().SerializeData(serializableDataManager, TAM_SPEEDLIMITDATA_ID, TAMSpeedLimitManager.instance.GetAllLaneData());
            }
        }
    }
}
