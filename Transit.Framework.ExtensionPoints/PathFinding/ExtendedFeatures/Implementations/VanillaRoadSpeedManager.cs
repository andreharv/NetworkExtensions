using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Implementations
{
    public class VanillaRoadSpeedManager : IRoadSpeedManager
    {
        public float GetLaneSpeedLimit(ref NetSegment segment, NetInfo.Lane laneInfo)
        {
            return laneInfo.m_speedLimit;
        }
    }
}
