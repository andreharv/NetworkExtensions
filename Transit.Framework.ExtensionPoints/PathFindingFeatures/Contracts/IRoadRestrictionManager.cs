using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts
{
    public interface IRoadRestrictionManager : IPathFindFeature
    {
		/// Determines if the lane with the given lane index and segment id may be used by the unit with the given type.
		/// 
		/// <param name="segmentId"></param>
		/// <param name="segmentInfo"></param>
		/// <param name="laneIndex"></param>
		/// <param name="laneId"></param>
		/// <param name="laneInfo"></param>
		/// <param name="unitType"></param>
		/// <returns>true if the lane may be used, else false</returns>
		bool CanUseLane(ushort segmentId, NetInfo segmentInfo, byte laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedUnitType unitType);
	}
}
