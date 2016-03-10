using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts
{
    public interface IRoadRestrictionManager : IPathFindFeature
    {
        bool CanUseLane(uint laneId, ExtendedUnitType unitType);
    }
}
