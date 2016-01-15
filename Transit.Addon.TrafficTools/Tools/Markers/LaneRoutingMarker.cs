using UnityEngine;

namespace Transit.Addon.TrafficTools.Tools.Markers
{
    class LaneRoutingMarker
    {
        public uint LaneId { get; private set; }
        public Vector3 Position { get; private set; }
        public bool IsSource { get; private set; }
        public float Size { get; private set; }
        public Color Color { get; private set; }
        public FastList<LaneRoutingMarker> Connections { get; private set; }

        public LaneRoutingMarker(uint laneId, Vector3 position, bool isSource, Color color)
        {
            LaneId = laneId;
            Position = position;
            IsSource = isSource;
            Size = 1f;
            Color = color;
            Connections = new FastList<LaneRoutingMarker>();
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
