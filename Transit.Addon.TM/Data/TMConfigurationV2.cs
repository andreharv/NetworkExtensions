using System;
using System.Collections.Generic;

namespace Transit.Addon.TM.Data
{
    [Serializable]
    public partial class TMConfigurationV2
    {
		/// <summary>
		/// Node configurations
		/// </summary>
		public List<NodeConf> NodeConfs = new List<NodeConf>();

		/// <summary>
		/// Segment configurations
		/// </summary>
		public List<SegmentConf> SegmentConfs = new List<SegmentConf>();

		/// <summary>
		/// Stored lane speed limits
		/// </summary>
		public List<LaneConf> LaneConfs = new List<LaneConf>();

		/// <summary>
		/// Options
		/// </summary>
		public Options Opt = new Options();
    }
}
