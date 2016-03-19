using System;

namespace Transit.Addon.TM.Data {
	public partial class TMConfigurationV2 {
		[Serializable]
		public class SegmentConf {
			public ushort segmentId;
			public SegmentEndConf startNodeConf = null;
			public SegmentEndConf endNodeConf = null;

			public SegmentConf() {
			}

			public SegmentConf(ushort segmentId) {
				this.segmentId = segmentId;
			}
		}
	}
}
