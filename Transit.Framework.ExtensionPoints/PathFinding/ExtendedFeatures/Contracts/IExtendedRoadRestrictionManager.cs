
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts
{
    public interface IExtendedRoadRestrictionManager
    {
        bool CanUseLane(uint laneId, ExtendedVehicleType vehicleType);
    }
}
