using System;

namespace Transit.Addon.TM.Data
{
    public partial class TMConfiguration
    {
        [Serializable]
        public class SegmentNodeConf
        {
            public ushort segmentId;
            public SegmentNodeFlags startNodeFlags = null;
            public SegmentNodeFlags endNodeFlags = null;

            public SegmentNodeConf(ushort segmentId)
            {
                this.segmentId = segmentId;
            }
        }
    }
}
