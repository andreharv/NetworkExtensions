using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts
{
    public interface ILaneRoutingManager : IPathFindFeature
    {
		/// <summary>
		/// Determines if units can switch<para/>
		/// from ORIGIN LANE (specified by originSegmentId, originLaneIndex, originLaneId)<para/>
		/// to DESTINATION LANE (specified by destinationSegmentId, destinationLaneIndex, destinationLaneId).
		/// </summary>
		/// <param name="nodeId"></param>
		/// <param name="originSegmentId"></param>
		/// <param name="originLaneIndex"></param>
		/// <param name="originLaneId"></param>
		/// <param name="destinationSegmentId"></param>
		/// <param name="destinationLaneIndex"></param>
		/// <param name="destinationLaneId"></param>
		/// <param name="unitType"></param>
		bool CanLanesConnect(
            ushort nodeId,

			ushort originSegmentId,
			byte originLaneIndex,
			uint originLaneId,

			ushort destinationSegmentId,
            byte destinationLaneIndex,
            uint destinationLaneId,

            ExtendedUnitType unitType);
	}
}
