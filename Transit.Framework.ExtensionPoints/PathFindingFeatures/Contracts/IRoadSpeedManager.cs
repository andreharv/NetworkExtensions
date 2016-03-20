using ColossalFramework;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts
{
    public interface IRoadSpeedManager : IPathFindFeature
    {
        float GetLaneSpeedLimit(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedUnitType unitType);
    }

    public static class ExtendedRoadSpeedManagerExtensions
    {
        public static float GetLaneSpeedLimit(this IRoadSpeedManager roadSpeedManager, ushort segmentId, uint laneIndex, ExtendedUnitType unitType)
        {
            var segment = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId];
            var laneInfo = segment.Info.m_lanes[laneIndex];
            var laneId = Singleton<NetManager>.instance.GetLaneId(laneInfo, ref segment);

            if(laneId == null)
            {
                throw new System.Exception("TFW: LaneId has not been found");
            }

            return roadSpeedManager.GetLaneSpeedLimit(segmentId, laneIndex, laneId.Value, laneInfo, unitType);
        }
    }
}
