
namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public interface ILaneRoutingManager
    {
        bool CanLanesConnect(ushort nodeId, uint laneId1, uint laneId2);
    }
}
