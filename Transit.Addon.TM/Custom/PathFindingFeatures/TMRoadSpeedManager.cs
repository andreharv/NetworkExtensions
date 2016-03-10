using System;
using TrafficManager.Traffic;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace TrafficManager.Custom.PathFindingFeatures
{
    public class TMRoadSpeedManager : IRoadSpeedManager
    {
        public float GetLaneSpeedLimit(ref NetSegment segment, NetInfo.Lane laneInfo, ExtendedUnitType unitType)
        {
            if ((unitType & TMSupported.UNITS) == 0)
            {
                return laneInfo.m_speedLimit;
            }

            var laneId = NetManager.instance.GetLaneId(laneInfo, segment);
            if (laneId == null)
            {
                throw new Exception("TAM: LaneId has not been found");
            }

            var laneIndex = NetManager.instance.GetLaneIndex(laneId.Value);
            if (laneIndex == null)
            {
                throw new Exception("TAM: LaneIndex has not been found");
            }

            var segmentId = NetManager.instance.GetLaneNetSegmentId(laneId.Value);

            return SpeedLimitManager.GetLockFreeGameSpeedLimit(segmentId, laneIndex.Value, laneId.Value, laneInfo);
        }
    }
}
