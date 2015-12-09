
namespace Transit.Addon.TrafficPP.PathFinding
{
	/*
	 * This is the class responsible for pathfinding. It's all in here since none of the methods can be overwritten.
	 * There's a lot of small changes here and there to make it generate a correct path for the service vehicles using pedestrian paths.
	 */
	partial class CustomPathFind : PathFind
	{
		private struct BufferItem
		{
			public PathUnit.Position m_position;
			public float m_comparisonValue;
			public float m_methodDistance;
			public uint m_laneID;
			public NetInfo.Direction m_direction;
			public NetInfo.LaneType m_lanesUsed;
		}

        private CustomPathFind.BufferItem[] m_buffer;
	}
}
