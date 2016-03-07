
namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Vanilla
{
    public class VanillaLaneRoutingManager : IExtendedLaneRoutingManager
    {
        public bool CanLanesConnect(ushort nodeId, uint laneId1, uint laneId2)
        {
            return true;
        }
    }
}
