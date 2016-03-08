
using ColossalFramework;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts
{
    public interface IRoadSpeedManager
    {
        float GetLaneSpeedLimit(ref NetSegment segment, NetInfo.Lane laneInfo);
    }

    public static class ExtendedRoadSpeedManagerExtensions
    {
        public static float GetLaneSpeedLimit(this IRoadSpeedManager roadSpeedManager, ushort segmentId, NetInfo.Lane laneInfo)
        {
            var segment = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId];

            return roadSpeedManager.GetLaneSpeedLimit(ref segment, laneInfo);
        }
    }
}
