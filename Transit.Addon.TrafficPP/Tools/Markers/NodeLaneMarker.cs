using UnityEngine;

namespace Transit.Addon.TrafficPP.Tools.Markers
{
    class NodeLaneMarker
    {
        public ushort NodeId { get; private set; }
        public uint LaneId { get; private set; }
        public Vector3 Position { get; private set; }
        public bool IsSource { get; private set; }
        public float Size { get; private set; }
        public Color Color { get; private set; }
        public FastList<NodeLaneMarker> Connections { get; private set; }

        public NodeLaneMarker(ushort nodeId, uint laneId, Vector3 position, bool isSource, Color color)
        {
            NodeId = nodeId;
            LaneId = laneId;
            Position = position;
            IsSource = isSource;
            Size = 1f;
            Color = color;
            Connections = new FastList<NodeLaneMarker>();
        }

        public void Select()
        {
            Size = 2f;
        }

        public void Unselect()
        {
            Size = 1f;
        }
    }
}
