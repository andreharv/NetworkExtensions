using System.Collections.Generic;
using Transit.Addon.TM.Tools.LaneRouting;
using Transit.Framework.Builders;
using Transit.Framework.UI.Toolbar.Infos;

namespace Transit.Addon.TM.UI.Toolbar.RoadEditor
{
    public class RoadEditorMenuMainCategoryInfo : IMenuCategoryInfo
    {
        public const string NAME = "RoadEditorMainCategory";

        public string Name { get { return NAME; } }
        public int Order { get { return 10; } }
        public IEnumerable<IToolBuilder> ToolBuilders { get; private set; }

        public RoadEditorMenuMainCategoryInfo()
        {
            // TODO: invert that dependency, RoadEditorMainCategoryInfo should fetch the ToolBuilder from the module
            ToolBuilders = new[]
            {
                new LaneRoutingToolBuilder(), 
            };
        }
    }
}
