
namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Vanilla
{
    public class VanillaLaneSpeedManager : IExtendedLaneSpeedManager
    {
        public float GetLaneSpeedLimit(NetInfo.Lane lane, uint laneId)
        {
            return lane.m_speedLimit;
        }
    }
}
