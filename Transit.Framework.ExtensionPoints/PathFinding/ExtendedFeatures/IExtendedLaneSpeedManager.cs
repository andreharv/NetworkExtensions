
namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures
{
    public interface IExtendedLaneSpeedManager
    {
        float GetLaneSpeedLimit(NetInfo.Lane lane, uint laneId);
    }
}
