using System;
using System.Collections.Generic;

namespace Transit.Addon.ToolsV2.LaneRouting.Data
{
    [Serializable]
    public class NodeRoutingData
    {
        public ushort NodeId { get; set; }
        public List<LaneRoutingData> Routes { get; set; }

        public NodeRoutingData()
        {
            Routes = new List<LaneRoutingData>();
        }
    }
}
