using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Implementations
{
    public class VanillaRoadSpeedManager : IRoadSpeedManager
    {
        public float GetLaneSpeedLimit(ref NetSegment segment, NetInfo.Lane laneInfo, ExtendedUnitType unitType)
        {
            return laneInfo.m_speedLimit;
        }
    }
}
