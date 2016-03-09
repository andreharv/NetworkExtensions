using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Implementations
{
    public class VanillaRoadSpeedManager : IRoadSpeedManager
    {
        public float GetLaneSpeedLimit(ref NetSegment segment, NetInfo.Lane laneInfo, ExtendedUnitType unitType)
        {
            return laneInfo.m_speedLimit;
        }
    }
}
