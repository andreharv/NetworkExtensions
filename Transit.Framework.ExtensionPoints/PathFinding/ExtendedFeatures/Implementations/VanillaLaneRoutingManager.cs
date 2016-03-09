using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Implementations
{
    public class VanillaLaneRoutingManager : ILaneRoutingManager
    {
        public bool CanLanesConnect(ushort nodeId, uint originLaneId, uint destinationLaneId, ExtendedUnitType vehicleType)
        {
            return true;
        }
    }
}
