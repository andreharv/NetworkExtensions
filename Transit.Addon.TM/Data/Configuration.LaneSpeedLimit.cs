using System;

namespace TrafficManager
{
    public partial class Configuration
    {
        [Serializable]
        public class LaneSpeedLimit
        {
            public uint laneId;
            public ushort speedLimit;

            public LaneSpeedLimit(uint laneId, ushort speedLimit)
            {
                this.laneId = laneId;
                this.speedLimit = speedLimit;
            }
        }
    }
}
