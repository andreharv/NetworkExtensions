using System;
using Transit.Framework.Builders;

namespace Transit.Addon.TM.UI.Toolbar.RoadEditor
{
    public class RoadEditorToolbarItemBuilder : IToolbarItemBuilder
    {
        public const string NAME = "RoadEditor";

        public string Name { get { return NAME; } }
        public string Description { get { return NAME; } }
        public int Order { get { return 11; } }
        public IMenuBuilder MenuBuilder { get; private set; }

        public RoadEditorToolbarItemBuilder()
        {
            MenuBuilder = new RoadEditorMenuBuilder();
        }
    }
}
