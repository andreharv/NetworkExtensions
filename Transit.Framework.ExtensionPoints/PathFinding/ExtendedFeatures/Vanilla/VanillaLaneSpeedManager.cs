
namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Vanilla
{
    public class VanillaLaneSpeedManager : IExtendedLaneSpeedManager
    {
        public float GetLaneSpeedLimit(ref NetSegment segment, NetInfo.Lane laneInfo)
        {
            return laneInfo.m_speedLimit;
        }
    }
}
