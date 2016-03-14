using System;
using System.Collections.Generic;

namespace TrafficManager
{
    public partial class Configuration
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
