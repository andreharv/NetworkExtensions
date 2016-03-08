
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures
{
    public interface IExtendedLaneRestrictionManager
    {
        bool CanUseLane(uint laneId, ExtendedVehicleType vehicleType);
    }
}
