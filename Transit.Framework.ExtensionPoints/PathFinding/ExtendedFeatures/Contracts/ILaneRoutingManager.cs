
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts
{
    public interface ILaneRoutingManager
    {
        bool CanLanesConnect(ushort nodeId, uint originLaneId, uint destinationLaneId, ExtendedUnitType vehicleType);
    }
}
