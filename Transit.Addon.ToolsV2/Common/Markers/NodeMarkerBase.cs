using System;
using UnityEngine;

namespace Transit.Addon.ToolsV2.Common.Markers
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
