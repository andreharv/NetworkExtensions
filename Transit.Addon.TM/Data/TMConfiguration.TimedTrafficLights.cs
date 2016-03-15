using System;
using System.Collections.Generic;

namespace Transit.Addon.TM.Data
{
    public partial class TMConfiguration
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
