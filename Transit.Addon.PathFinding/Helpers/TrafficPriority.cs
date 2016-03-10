using ColossalFramework;
using UnityEngine;

namespace Transit.Addon.PathFinding.Helpers {
	class TrafficPriority {
		
		/// <summary>
		/// Determines if the map uses a left-hand traffic system
		/// </summary>
		/// <returns></returns>
		public static bool IsLeftHandDrive() {
			return Singleton<SimulationManager>.instance.m_metaData.m_invertTraffic == SimulationMetaData.MetaBool.True;
		}

        public static bool IsRightSegment(ushort fromSegment, ushort toSegment, ushort nodeid)
        {
            if (fromSegment <= 0 || toSegment <= 0)
                return false;

            return IsLeftSegment(toSegment, fromSegment, nodeid);
        }

        public static bool IsLeftSegment(ushort fromSegment, ushort toSegment, ushort nodeid)
        {
            if (fromSegment <= 0 || toSegment <= 0)
                return false;

            Vector3 fromDir = GetSegmentDir(fromSegment, nodeid);
            fromDir.y = 0;
            fromDir.Normalize();
            Vector3 toDir = GetSegmentDir(toSegment, nodeid);
            toDir.y = 0;
            toDir.Normalize();
            return Vector3.Cross(fromDir, toDir).y >= 0.5;
        }

        public static Vector3 GetSegmentDir(int segment, ushort nodeid)
        {
            var instance = Singleton<NetManager>.instance;

            Vector3 dir;

            dir = instance.m_segments.m_buffer[segment].m_startNode == nodeid ?
                instance.m_segments.m_buffer[segment].m_startDirection :
                instance.m_segments.m_buffer[segment].m_endDirection;

            return dir;
        }
    }
}
