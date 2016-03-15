using System;

namespace Transit.Addon.TM.Data
{
    public partial class TMConfiguration
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
