
namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures
{
    public interface IExtendedLaneRoutingManager
    {
        bool CanLanesConnect(ushort nodeId, uint laneId1, uint laneId2);
    }
}
