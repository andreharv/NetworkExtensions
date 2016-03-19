using System;
using System.Collections.Generic;

namespace Transit.Addon.TM.Data
{
    public partial class TMConfigurationV2
    {
        [Serializable]
        public class TimedTrafficLights
        {
            public List<ushort> nodeGroup;
            public bool started;
            public List<TimedTrafficLightsStep> timedSteps;
        }
    }
}
