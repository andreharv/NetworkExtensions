using System;

namespace Transit.Addon.TM.Events
{
    public delegate void NetNodesUpdatedEventHandler(NetNodesUpdatedEventArgs e);

    public class NetNodesUpdatedEventArgs : EventArgs
    {
        public ushort[] NodeIds { get; private set; }

        public NetNodesUpdatedEventArgs(ushort[] nodeIds)
        {
            NodeIds = nodeIds;
        }
    }
}