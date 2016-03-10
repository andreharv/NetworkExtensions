using System;
using System.Collections.Generic;

namespace Transit.Addon.PathFinding.Data
{
    [Serializable]
    public class Configuration
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
