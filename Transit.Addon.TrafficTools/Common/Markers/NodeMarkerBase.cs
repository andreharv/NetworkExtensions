using System;
using UnityEngine;

namespace Transit.Addon.TrafficTools.Common.Markers
{
    public abstract class NodeMarkerBase : UIMarkerBase
    {
        public ushort NodeId { get; private set; }

        protected NodeMarkerBase(ushort nodeId)
        {
            NodeId = nodeId;
        }
    }
}
