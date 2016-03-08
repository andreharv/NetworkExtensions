
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Vanilla
{
    public class VanillaLaneRestrictionManager : IExtendedLaneRestrictionManager
    {
        public bool CanUseLane(uint laneId, ExtendedVehicleType vehicleType)
        {
            return true;
        }
    }
}
