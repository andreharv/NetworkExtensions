using ColossalFramework;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts
{
    public interface ISpeedLimitManager : IPathFindFeature
    {
        float GetLaneSpeedLimit(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedUnitType unitType);
    }

    public static class ISpeedLimitManagerExtensions
    {
        public static float GetLaneSpeedLimit(this ISpeedLimitManager speedLimitManager, ushort segmentId, uint laneIndex, ExtendedUnitType unitType)
        {
            var segment = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId];
            var laneInfo = segment.Info.m_lanes[laneIndex];
            var laneId = Singleton<NetManager>.instance.GetLaneId(laneInfo, ref segment);

            if(laneId == null)
            {
                throw new System.Exception("TFW: LaneId has not been found");
            }

            return speedLimitManager.GetLaneSpeedLimit(segmentId, laneIndex, laneId.Value, laneInfo, unitType);
        }
    }
}
