using System;

namespace Transit.Addon.ToolsV2.LaneRouting.Data
{
    [Serializable]
    public class LaneRoutingData
    {
        public ushort OriginSegmentId { get; set; }
        public uint OriginLaneId { get; set; }

        public ushort DestinationSegmentId { get; set; }
        public uint DestinationLaneId { get; set; }
    }
}
