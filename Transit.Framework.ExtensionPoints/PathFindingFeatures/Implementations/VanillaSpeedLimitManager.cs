using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Implementations
{
    public class VanillaSpeedLimitManager : ISpeedLimitManager
    {
        public float GetLaneSpeedLimit(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedUnitType unitType)
        {
            return laneInfo.m_speedLimit;
        }
    }
}
