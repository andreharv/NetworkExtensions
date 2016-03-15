using System;

namespace Transit.Addon.TM.Data
{
    public partial class TMConfigurationV2
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
