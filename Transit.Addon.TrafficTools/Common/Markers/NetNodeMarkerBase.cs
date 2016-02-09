using System;
using UnityEngine;

namespace Transit.Addon.TrafficTools.Common.Markers
{
    public abstract class NetNodeMarkerBase : UIMarkerBase
    {
        public ushort NodeId { get; private set; }

        protected NetNodeMarkerBase(ushort nodeId)
        {
            NodeId = nodeId;
        }
    }
}
