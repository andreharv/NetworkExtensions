using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts
{
    public interface ILaneRoutingManager : IPathFindFeature
    {
		// Warning: Avoid using this method due to performance considerations.
		bool CanLanesConnect(ushort nodeId, uint originLaneId, uint destinationLaneId, ExtendedUnitType unitType);

		/// <summary>
		/// Determines if units by switch from one lane1 (specified by segment1Id, lane1Index, lane1Id) to lane2 (specified by segment2Id, lane2Index, lane2Id).
		/// </summary>
		/// <param name="nodeId"></param>
		/// <param name="segment1Id"></param>
		/// <param name="lane1Index"></param>
		/// <param name="lane1Id"></param>
		/// <param name="segment2Id"></param>
		/// <param name="lane2Index"></param>
		/// <param name="lane2Id"></param>
		/// <param name="unitType"></param>
		/// <returns></returns>
		bool CanLanesConnect(ushort nodeId, ushort segment1Id, byte lane1Index, uint lane1Id, ushort segment2Id, byte lane2Index, uint lane2Id, ExtendedUnitType unitType);
	}
}
