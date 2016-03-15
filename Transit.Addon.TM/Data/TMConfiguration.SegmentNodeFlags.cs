using System;

namespace Transit.Addon.TM.Data
{
    public partial class TMConfiguration
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
