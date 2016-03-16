using System;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Implementations
{
    public class VanillaLaneRoutingManager : ILaneRoutingManager
    {
        public bool CanLanesConnect
           (ushort nodeId,

			ushort originSegmentId,
			byte originLaneIndex,
			uint originLaneId,

			ushort destinationSegmentId,
            byte destinationLaneIndex,
            uint destinationLaneId, 

            ExtendedUnitType unitType)
        {
			return true;
		}
	}
}
