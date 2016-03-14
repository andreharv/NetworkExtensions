using System;
using System.Collections.Generic;

namespace TrafficManager
{
    public partial class Configuration
    {
        [Serializable]
        public class SegmentNodeFlags
        {
            public bool? uturnAllowed = null;
            public bool? straightLaneChangingAllowed = null;
            public bool? enterWhenBlockedAllowed = null;
        }
    }
}
