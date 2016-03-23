using System;
using Transit.Framework.UI.Toolbar.Infos;

namespace Transit.Addon.TM.UI.Toolbar.RoadEditor
{
    public class RoadEditorToolbarItemInfo : IToolbarItemInfo
    {
        public const string NAME = "RoadEditor";

        public string Name { get { return NAME; } }
        public string Description { get { return NAME; } }
        public int Order { get { return 11; } }
        public IMenuInfo MenuInfo { get; private set; }

        public RoadEditorToolbarItemInfo()
        {
            MenuInfo = new RoadEditorMenuInfo();
        }
    }
}
