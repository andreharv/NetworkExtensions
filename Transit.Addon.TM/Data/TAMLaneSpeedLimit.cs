using System;

namespace Transit.Addon.TM.Data
{
    [Serializable]
    public class TAMLaneSpeedLimit
    {
        public uint LaneId { get; set; }
        public ushort? SpeedLimit { get; set; }

        public TAMLaneSpeedLimit()
        {
            SpeedLimit = 1;
        }

        public override string ToString()
        {
            return string.Format("Speed limit on lane {0} of {1}", LaneId, (SpeedLimit == null ? "Unset" : SpeedLimit.Value.ToString() + " km/h"));
        }
    }
}
