using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Implementations
{
    public class VanillaRoadRestrictionManager : IRoadRestrictionManager
    {
        public bool CanUseLane(uint laneId, ExtendedUnitType vehicleType)
        {
            return true;
        }
    }
}
