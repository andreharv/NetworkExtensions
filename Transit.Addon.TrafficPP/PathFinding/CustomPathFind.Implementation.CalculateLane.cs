using ColossalFramework;
using Transit.Addon.TrafficPP.Core;

namespace Transit.Addon.TrafficPP.PathFinding
{
	partial class CustomPathFind : PathFind
	{
        private static void CalculateLaneDirection(PathUnit.Position pathPos, out NetInfo.Direction direction, out NetInfo.LaneType type)
		{
			NetManager instance = Singleton<NetManager>.instance;
			//if (instance == null)
			//	Logger.LogInfo("GetLaneDirection -> instance is null!\n");
			NetInfo info = instance.m_segments.m_buffer[(int)pathPos.m_segment].Info;
			//if (info == null)
			//	Logger.LogInfo("GetLaneDirection -> info is null!\n");
			//else if (info.m_lanes == null)
			//	Logger.LogInfo("GetLaneDirection -> info.m_lanes is null!\n");
			if (info.m_lanes.Length > (int)pathPos.m_lane)
			{
				direction = info.m_lanes[(int)pathPos.m_lane].m_finalDirection;
				type = info.m_lanes[(int)pathPos.m_lane].m_laneType;
				if ((instance.m_segments.m_buffer[(int)pathPos.m_segment].m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None)
				{
					direction = NetInfo.InvertDirection(direction);
				}
			}
			else
			{
				direction = NetInfo.Direction.None;
				type = NetInfo.LaneType.None;
			}
		}

        private static float CalculateLaneSpeed(byte startOffset, byte endOffset, NetSegment segment, NetInfo.Lane laneInfo, uint laneId)
        {
            float speedLimit = (TrafficPPModule.ActiveOptions & TrafficPPModule.ModOptions.RoadCustomizerTool) == TrafficPPModule.ModOptions.RoadCustomizerTool ? LanesManager.GetLaneSpeed(laneId) : laneInfo.m_speedLimit;
            //float speedLimit = laneInfo.m_speedLimit;

            NetInfo.Direction direction = ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ? laneInfo.m_finalDirection : NetInfo.InvertDirection(laneInfo.m_finalDirection);
            if ((byte)(direction & NetInfo.Direction.Avoid) == 0)
            {
                //return laneInfo.m_speedLimit;
                return speedLimit;
            }
            if (endOffset > startOffset && direction == NetInfo.Direction.AvoidForward)
            {
                //return laneInfo.m_speedLimit * 0.1f;
                return speedLimit * 0.1f;
            }
            if (endOffset < startOffset && direction == NetInfo.Direction.AvoidBackward)
            {
                //return laneInfo.m_speedLimit * 0.1f;
                return speedLimit * 0.1f;
            }
            //return laneInfo.m_speedLimit * 0.2f;
            return speedLimit * 0.2f;
        }
	}
}
