using System;
using System.Collections.Generic;

namespace TrafficManager
{
    public partial class Configuration
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
