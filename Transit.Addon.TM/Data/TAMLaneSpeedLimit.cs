using System;

namespace Transit.Addon.TM.Data
{
    [Serializable]
    public class TAMLaneSpeedLimit
    {
        public uint LaneId { get; set; }
        public float SpeedLimit { get; set; }

        public TAMLaneSpeedLimit()
        {
            SpeedLimit = 1f;
        }
    }
}
