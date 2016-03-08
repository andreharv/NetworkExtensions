
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts
{
    public interface IExtendedLaneRoutingManager
    {
        bool CanLanesConnect(ushort nodeId, uint originLaneId, uint destinationLaneId, ExtendedVehicleType vehicleType);
    }
}
