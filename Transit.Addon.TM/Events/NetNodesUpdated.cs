using System;

namespace Transit.Addon.TM.Events
{
    public delegate void NetNodesUpdatedEventHandler(NetNodesUpdatedEventArgs e);

    public class NetNodesUpdatedEventArgs : EventArgs
    {
        public ulong[] NodeIds { get; private set; }

        public NetNodesUpdatedEventArgs(ulong[] nodeIds)
        {
            NodeIds = nodeIds;
        }
    }
}