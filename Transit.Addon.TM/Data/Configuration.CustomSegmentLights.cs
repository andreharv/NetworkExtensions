using System;
using System.Collections.Generic;

namespace TrafficManager
{
    public partial class Configuration
    {
        [Serializable]
        public class CustomSegmentLights
        {
            public ushort nodeId;
            public ushort segmentId;
            public Dictionary<Traffic.ExtVehicleType, CustomSegmentLight> customLights;
            public RoadBaseAI.TrafficLightState? pedestrianLightState;
            public bool manualPedestrianMode;
        }
    }
}
