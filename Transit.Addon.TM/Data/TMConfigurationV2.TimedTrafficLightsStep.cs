using System;
using System.Collections.Generic;

namespace Transit.Addon.TM.Data
{
    public partial class TMConfigurationV2
    {
        [Serializable]
        public class TimedTrafficLightsStep
        {
            public int minTime;
            public int maxTime;
            public float waitFlowBalance;
            public Dictionary<ushort, CustomSegmentLights> segmentLights;
        }
    }
}
