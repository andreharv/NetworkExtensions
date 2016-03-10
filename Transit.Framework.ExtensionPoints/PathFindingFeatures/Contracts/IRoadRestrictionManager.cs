using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts
{
    public interface IRoadRestrictionManager : IPathFindFeature
    {
        /// <param name="laneId"></param>
        /// <param name="segmentId">Optional information specifying where the laneId comes from</param>
        /// <param name="laneIndex">Optional information specifying where the laneId comes from</param>
        /// <param name="unitType"></param>
        /// <returns>Returns whenever the lane can be use of not</returns>
        bool CanUseLane(uint laneId, ushort? segmentId, uint? laneIndex, ExtendedUnitType unitType);
    }
}
