using System;
using Transit.Addon.TM.Traffic;

namespace Transit.Addon.TM.Data {
	public partial class TMConfigurationV2 {
		[Serializable]
		public class SegmentEndFlags {
			public bool? uturnAllowed = null;
			public bool? straightLaneChangingAllowed = null;
			public bool? enterWhenBlockedAllowed = null;
		}
	}
}
