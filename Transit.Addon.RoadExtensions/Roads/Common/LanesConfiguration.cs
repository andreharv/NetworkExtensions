
namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public class LanesConfiguration
    {
        public int LanesToAdd { get; set; }
        public float PedPropOffsetX { get; set; }
        public float? SpeedLimit { get; set; }
        public float LaneWidth { get; set; }
        public bool IsTwoWay { get; set; }
        public CenterLaneType CenterLane { get; set; }

        public LanesConfiguration()
        {
            LanesToAdd = 0;
            PedPropOffsetX = 0.0f;
            SpeedLimit = null;
            LaneWidth = 3.0f;
            IsTwoWay = false;
            CenterLane = CenterLaneType.None;
        }
    }
}
