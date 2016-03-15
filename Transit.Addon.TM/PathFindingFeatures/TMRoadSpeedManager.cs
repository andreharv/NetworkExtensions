using Transit.Addon.TM.Traffic;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public class TMRoadSpeedManager : IRoadSpeedManager
    {
        public float GetLaneSpeedLimit(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedUnitType unitType)
        {
            if ((unitType & TMSupported.UNITS) == 0)
            {
                return laneInfo.m_speedLimit;
            }

            return SpeedLimitManager.GetLockFreeGameSpeedLimit(segmentId, laneIndex, laneId, laneInfo);
        }
    }
}
