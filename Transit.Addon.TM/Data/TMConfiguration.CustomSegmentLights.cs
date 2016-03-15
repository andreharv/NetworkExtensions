using System;
using System.Collections.Generic;
using Transit.Addon.TM.Traffic;

namespace Transit.Addon.TM.Data
{
    public partial class TMConfiguration
    {
        [Serializable]
        public class CustomSegmentLights
        {
            public ushort nodeId;
            public ushort segmentId;
            public Dictionary<TMVehicleType, CustomSegmentLight> customLights;
            public RoadBaseAI.TrafficLightState? pedestrianLightState;
            public bool manualPedestrianMode;
        }
    }
}
