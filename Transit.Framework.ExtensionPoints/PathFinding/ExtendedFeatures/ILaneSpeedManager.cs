
namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public interface ILaneSpeedManager
    {
        float GetLaneSpeedLimit(NetInfo.Lane lane, uint laneId);
    }
}
