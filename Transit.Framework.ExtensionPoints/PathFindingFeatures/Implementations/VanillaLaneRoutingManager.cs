using System;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Implementations
{
    public class VanillaLaneRoutingManager : ILaneRoutingManager
    {
        public bool CanLanesConnect
           (ushort nodeId,

            ushort destinationSegmentId,
            byte destinationLaneIndex,
            uint destinationLaneId, 

            ushort originSegmentId, 
            byte originLaneIndex, 
            uint originLaneId, 

            ExtendedUnitType unitType)
        {
			return true;
		}
	}
}
