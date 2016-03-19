using TrafficManager;
using Transit.Addon.TM.AI;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.Traffic;

namespace Transit.Addon.TM.Data {
	public static partial class TMDataManager {
		public static TMConfigurationV2.Options Options {
			get; private set;
		} = new TMConfigurationV2.Options();

		private static void OnBeforeLoad() {
			Flags.OnBeforeLoadData();
			CustomRoadAI.OnBeforeLoadData();
		}

		private static void OnAfterLoad() {
			Flags.clearHighwayLaneArrows();
			Flags.applyAllFlags();
			TrafficPriority.HandleAllVehicles();
		}

		internal static void Apply(TMConfigurationV2 configuration) {
			OnBeforeLoad();
			if (configuration != null) {
				if (configuration.Opt != null)
					Options = configuration.Opt;
				ApplyConfiguration(configuration);
			}
			OnAfterLoad();
		}

		internal static void Apply(Configuration dataV1, byte[] options) {
			OnBeforeLoad();
			Options = ParseV1Options(options);
			ApplyConfiguration(dataV1);
			OnAfterLoad();
		}
	}
}
