using Transit.Addon.TrafficPP.Core;

namespace Transit.Addon.TrafficPP.PathFinding
{
    partial class CustomPathFind : PathFind
	{
		private float CalculateLaneSpeed(byte startOffset, byte endOffset, NetSegment segment, NetInfo.Lane laneInfo, uint laneId)
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
