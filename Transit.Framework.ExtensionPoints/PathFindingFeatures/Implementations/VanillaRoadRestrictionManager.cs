using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Implementations
{
    public class VanillaRoadRestrictionManager : IRoadRestrictionManager
    {
        public bool CanUseLane(uint laneId, ExtendedUnitType vehicleType)
        {
            return true;
        }
    }
}
