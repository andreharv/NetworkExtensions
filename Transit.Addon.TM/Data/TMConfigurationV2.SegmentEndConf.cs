using System;
using Transit.Addon.TM.Traffic;

namespace Transit.Addon.TM.Data {
	public partial class TMConfigurationV2 {
		[Serializable]
		public class SegmentEndConf {
			public SegmentEndFlags flags = null;
			public SegmentEnd.PriorityType? priorityType = null;
		}
	}
}
