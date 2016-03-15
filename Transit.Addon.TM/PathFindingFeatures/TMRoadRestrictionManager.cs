using Transit.Addon.TM.Data;
using Transit.Addon.TM.Traffic;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public class TMRoadRestrictionManager : IRoadRestrictionManager
    {
        public bool CanUseLane(ushort segmentId, byte laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedUnitType unitType)
        {
            if ((unitType & TMSupported.UNITS) == 0)
            {
                // unsupported type. allow.
                return true;
            }

            TMVehicleType allowedVehicleTypes = VehicleRestrictionsManager.GetAllowedVehicleTypes(segmentId, laneIndex, laneInfo);
            ExtendedUnitType allowedUnitTypes = allowedVehicleTypes.ConvertToUnitType(); // TODO remove type TMVehicleType

            return ((allowedUnitTypes & unitType) != 0);
        }
    }
}
