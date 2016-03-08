
using ColossalFramework;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures
{
    public interface IExtendedLaneSpeedManager
    {
        float GetLaneSpeedLimit(ref NetSegment segment, NetInfo.Lane laneInfo);
    }

    public static class ExtendedLaneSpeedManagerExtensions
    {
        public static float GetLaneSpeedLimit(this IExtendedLaneSpeedManager laneSpeedManager, ushort segmentId, NetInfo.Lane laneInfo)
        {
            var segment = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId];

            return laneSpeedManager.GetLaneSpeedLimit(ref segment, laneInfo);
        }
    }
}
