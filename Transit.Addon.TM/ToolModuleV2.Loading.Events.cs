using Transit.Addon.TM.Events;
using Transit.Addon.TM.Events.Managers;
using Transit.Framework;

namespace Transit.Addon.TM
{
    public partial class ToolModuleV2
    {
        private void InstallEventHandlers()
        {
            NetEventManager.instance.NetNodesUpdated += NetEventManager_NetNodesUpdated;
        }

        private void UninstallEventHandlers()
        {
            NetEventManager.instance.NetNodesUpdated -= NetEventManager_NetNodesUpdated;
        }

        private void NetEventManager_NetNodesUpdated(NetNodesUpdatedEventArgs e)
        {
            Log.Info(">>>>> NetNodesUpdated");

            foreach (var nodeId in e.NodeIds)
            {
                Log.Info(">>>>> NetNode " + nodeId);
            }
        }
    }
}
