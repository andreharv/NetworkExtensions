using System;
using System.Collections.Generic;

namespace TrafficManager
{
    public partial class Configuration
    {
        [Serializable]
        public class TimedTrafficLights
        {
            public ushort nodeId;
            public List<ushort> nodeGroup;
            public bool started;
            public List<TimedTrafficLightsStep> timedSteps;
        }
    }
}
