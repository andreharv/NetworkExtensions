using System;
using Transit.Framework.Network;

namespace Transit.Addon.TM.Data
{
    [Serializable]
    public class TAMLaneRestriction
    {
        public uint LaneId { get; set; }
        public ExtendedUnitType? UnitTypes { get; set; }

        public TAMLaneRestriction()
        {
            UnitTypes = ExtendedUnitType.RoadVehicle;
        }

        public override string ToString()
        {
            return string.Format("Restriction on lane {0} of type {1}", LaneId, (UnitTypes == null ? "Unset" : UnitTypes.Value.ToString()));
        }
    }
}
