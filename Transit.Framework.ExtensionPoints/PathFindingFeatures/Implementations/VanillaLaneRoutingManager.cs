using System;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Implementations
{
    public class VanillaLaneRoutingManager : ILaneRoutingManager
    {
        public bool CanLanesConnect(ushort nodeId, uint originLaneId, uint destinationLaneId, ExtendedUnitType unitType)
        {
            return true;
        }

		public bool CanLanesConnect(ushort nodeId, ushort segment1Id, byte lane1Index, uint lane1Id, ushort segment2Id, byte lane2Index, uint lane2Id, ExtendedUnitType unitType) {
			return true;
		}
	}
}
