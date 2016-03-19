using System;
using Transit.Framework.Network;

namespace Transit.Addon.TM.Data
{
    [Serializable]
    public class TAMLaneRestriction
    {
        public uint LaneId { get; set; }
        public ExtendedUnitType UnitTypes { get; set; }

        public TAMLaneRestriction()
        {
            UnitTypes = ExtendedUnitType.RoadVehicle;
        }
    }
}
