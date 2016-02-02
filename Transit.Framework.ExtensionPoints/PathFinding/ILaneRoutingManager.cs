
namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public interface ILaneRoutingManager
    {
        bool CanLanesConnect(uint laneId1, uint laneId2);
    }
}
