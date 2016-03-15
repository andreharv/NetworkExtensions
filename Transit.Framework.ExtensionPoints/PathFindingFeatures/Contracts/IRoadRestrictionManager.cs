using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts
{
    public interface IRoadRestrictionManager : IPathFindFeature
    {
		/// Determines if the lane with the given id may be used by the unit with the given type.
		/// Warning: Avoid using this method due to performance considerations.
		/// 
		/// <param name="laneId"></param>
		/// <param name="unitType"></param>
		/// <returns>true if the lane may be used, else false</returns>
		bool CanUseLane(uint laneId, ExtendedUnitType unitType);

		/// Determines if the lane with the given lane index and segment id may be used by the unit with the given type.
		/// 
		/// <param name="segmentId"></param>
		/// <param name="laneIndex"></param>
		/// <param name="laneId"></param>
		/// <param name="laneInfo"></param>
		/// <param name="unitType"></param>
		/// <returns>true if the lane may be used, else false</returns>
		bool CanUseLane(ushort segmentId, byte laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedUnitType unitType);
	}
}
