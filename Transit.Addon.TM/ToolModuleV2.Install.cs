using ICities;
using Transit.Addon.TM.UI.Toolbar.RoadEditor;
using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework.Modularity;

namespace Transit.Addon.TM
{
    public partial class ToolModuleV2
    {
        public override void OnInstallingContent()
        {
            base.OnInstallingContent();

            if (!GameMenuManager.IsToolbarItemInstalled<RoadEditorToolbarItemInfo>())
            {
                GameMenuManager.AddToolbarItem<RoadEditorToolbarItemInfo>();
                GameMenuManager.AddBigSeparator(12);
            }
        }
    }
}
