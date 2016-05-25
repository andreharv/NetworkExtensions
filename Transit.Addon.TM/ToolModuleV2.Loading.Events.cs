using Transit.Addon.TM.Events;
using Transit.Addon.TM.Events.Managers;
using Transit.Addon.TM.Overlays.LaneRouting;
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
            if (NodeRoutesOverlay.instance.IsLoaded())
            {
                foreach (var nodeId in e.NodeIds)
                {
                    NodeRoutesOverlay.instance.ScrubMarker(nodeId);
                }
            }
        }
    }
}
