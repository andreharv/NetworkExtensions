using Transit.Addon.TM.Overlays.LaneRouting;

namespace Transit.Addon.TM
{
    public partial class ToolModuleV2
    {
        private void InstallOverlays()
        {
            NodeRoutesOverlay.instance.LoadMarkers();
        }

        private void UninstallOverlays()
        {
            NodeRoutesOverlay.instance.Reset();
        }
    }
}
