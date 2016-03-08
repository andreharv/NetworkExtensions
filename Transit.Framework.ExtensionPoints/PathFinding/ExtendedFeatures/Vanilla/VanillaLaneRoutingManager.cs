
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Vanilla
{
    public class VanillaLaneRoutingManager : IExtendedLaneRoutingManager
    {
        public bool CanLanesConnect(ushort nodeId, uint originLaneId, uint destinationLaneId, ExtendedVehicleType vehicleType)
        {
            return true;
        }
    }
}
