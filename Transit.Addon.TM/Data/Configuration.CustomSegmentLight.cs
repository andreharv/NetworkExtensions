using System;
using System.Collections.Generic;

namespace TrafficManager
{
    public partial class Configuration
    {
        [Serializable]
        public class CustomSegmentLight
        {
            public ushort nodeId;
            public ushort segmentId;
            public int currentMode;
            public RoadBaseAI.TrafficLightState leftLight;
            public RoadBaseAI.TrafficLightState mainLight;
            public RoadBaseAI.TrafficLightState rightLight;
        }
    }
}
