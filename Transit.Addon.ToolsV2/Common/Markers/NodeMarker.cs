using System;

namespace Transit.Addon.ToolsV2.Common.Markers
{
    public static class NodeMarker
    {
        public static TMarker Create<TMarker>(ushort nodeId)
            where TMarker : NodeMarkerBase
        {
            return (TMarker)Activator.CreateInstance(typeof(TMarker), new object[] { nodeId });
        }
    }
}
