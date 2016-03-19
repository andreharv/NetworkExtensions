using ICities;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TrafficManager;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Framework.Serialization;

namespace Transit.Addon.TM.DataSerialization {
	public class TMDataSerializer : SerializableDataExtensionBase {
		private const string DATAV1_ID = "TrafficManager_v1.0";
		private const string DATAV2_ID = "TrafficManager_v2.0";
		private const string OPTIONS_DATAV1_ID = "TMPE_Options";

		public static bool StateLoading = false;

		public override void OnLoadData() {
			Log.Info("Loading Traffic Manager: PE Data!");
			StateLoading = true;
			try {
				TMConfigurationV2 configuration = new DataSerializer<TMConfigurationV2>().DeserializeData(serializableDataManager, DATAV2_ID);
				if (configuration != null) {
					Log.Info("TM:PE data version 2");
					TMDataManager.Apply(configuration);
				} else {
					// load options
					byte[] options = serializableDataManager.LoadData(OPTIONS_DATAV1_ID);

					// load V1 configuration
					Configuration dataV1 = new DataSerializer<Configuration>().DeserializeData(serializableDataManager, DATAV1_ID);
					if (dataV1 != null) {
						Log.Info("TM:PE data version 1");
						TMDataManager.Apply(dataV1, options);
					} else {
						Log.Info("No TM:PE save data found.");
						TMDataManager.Apply(null);
					}
				}

				
			} catch (Exception e) {
				Log.Error($"OnLoadData: {e.ToString()}");
			} finally {
				StateLoading = false;
			}

			Log.Info("OnLoadData completed.");
		}

		public override void OnSaveData() {
			Log.Info("Saving Mod Data.");
			TMConfigurationV2 configuration = TMDataManager.CreateConfiguration();

			var binaryFormatter = new BinaryFormatter();
			var memoryStream = new MemoryStream();

			try {
				binaryFormatter.Serialize(memoryStream, configuration);
				memoryStream.Position = 0;
				Log.Info($"Save data byte length {memoryStream.Length}");
				serializableDataManager.SaveData(DATAV2_ID, memoryStream.ToArray());
			} catch (Exception ex) {
				Log.Error("Unexpected error saving data: " + ex.Message);
			} finally {
				memoryStream.Close();
			}
		}
	}
}
