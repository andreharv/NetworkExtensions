using System;

namespace Transit.Addon.TM.Data {
	public partial class TMConfigurationV2 {
		[Serializable]
		public class LaneConf {
			public uint laneId;
			public ushort? speedLimit = null;
			public TMVehicleType? vehicleTypes = null;
			public Flags.LaneArrows? laneArrows = null;

			public LaneConf() {
			}

			public LaneConf(uint laneId) {
				this.laneId = laneId;
			}
		}
	}
}
