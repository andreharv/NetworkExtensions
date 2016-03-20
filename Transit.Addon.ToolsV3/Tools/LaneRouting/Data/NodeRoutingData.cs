using System;
using System.Collections.Generic;
using System.Linq;

namespace Transit.Addon.ToolsV3.LaneRouting.Data
{
    [Serializable]
    public class NodeRoutingData
    {
        public ushort NodeId { get; set; }
        public List<LaneRoutingData> Routes { get; set; }

        [NonSerialized]
        private Dictionary<uint, Dictionary<uint, LaneRoutingData>> _indexedRoutes;
        private Dictionary<uint, Dictionary<uint, LaneRoutingData>> IndexedRoutes
        {
            get
            {
                if (_indexedRoutes == null)
                {
                    _indexedRoutes = Routes
                        .GroupBy(r => r.OriginLaneId)
                        .ToDictionary(orGroup => orGroup.Key, orGroup => orGroup.ToDictionary(r => r.DestinationLaneId));
                }

                return _indexedRoutes;
            }
        }

        public bool HasRoutes
        {
            get { return Routes == null || Routes.Count == 0; }
        }

        public NodeRoutingData()
        {
            Routes = new List<LaneRoutingData>();
        }
    }
}
