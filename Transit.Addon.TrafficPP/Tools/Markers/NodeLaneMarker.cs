using UnityEngine;

namespace Transit.Addon.TrafficPP.Tools.Markers
{
    class NodeLaneMarker
    {
        public ushort m_node;
        public Vector3 m_position;
        public bool m_isSource;
        public uint m_lane;
        public float m_size = 1f;
        public Color m_color;
        public FastList<NodeLaneMarker> m_connections = new FastList<NodeLaneMarker>();
    }
}
