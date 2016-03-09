
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts
{
    public interface IRoadRestrictionManager
    {
        bool CanUseLane(uint laneId, ExtendedUnitType unitType);
    }
}
