using System;

namespace Transit.Addon.TM.Data {
	public partial class TMConfigurationV2 {
		[Serializable]
		public class NodeConf {
			public ushort nodeId;
			public bool? hasTrafficLight = null;
			public TimedTrafficLights timedLights = null;

			public NodeConf(ushort nodeId) {
				this.nodeId = nodeId;
			}
		}
	}
}
