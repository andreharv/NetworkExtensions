using ColossalFramework;

namespace Transit.Addon.TM.Events.Managers
{
    public class NetEventManager : Singleton<NetEventManager>
    {
        private event NetNodesUpdatedEventHandler _netNodesUpdated;

        public event NetNodesUpdatedEventHandler NetNodesUpdated
        {
            add { _netNodesUpdated += value; }
            remove { _netNodesUpdated -= value; }
        }

        public void FireNetNodesUpdated(NetNodesUpdatedEventArgs e)
        {
            if (_netNodesUpdated != null)
            {
                _netNodesUpdated.Invoke(e);
            }
        }
    }
}