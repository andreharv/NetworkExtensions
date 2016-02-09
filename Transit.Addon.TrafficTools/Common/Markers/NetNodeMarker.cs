using System;

namespace Transit.Addon.TrafficTools.Common.Markers
{
    public static class NetNodeMarker
    {
        public static TMarker Create<TMarker>(ushort nodeId)
            where TMarker : NetNodeMarkerBase
        {
            return (TMarker)Activator.CreateInstance(typeof(TMarker), new object[] { nodeId });
        }
    }
}
