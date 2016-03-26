using System;

namespace Transit.Addon.TM.Data {
	public partial class TMConfigurationV2 {
		[Serializable]
		public class LaneConf {
			public uint laneId;
			public TAMLaneDirection? directions = null;

			public LaneConf() {
			}

			public LaneConf(uint laneId) {
				this.laneId = laneId;
			}
		}
	}
}
