using System;

namespace Transit.Addon.ToolsV2.LaneRouting.Data
{
    [Serializable]
    public class LaneRoutingData
    {
        public ushort OriginLaneId { get; set; }
        public ushort DestinationLaneId { get; set; }
    }
}
