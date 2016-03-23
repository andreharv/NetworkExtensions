using System.Collections.Generic;
using Transit.Framework.Builders;

namespace Transit.Addon.TM.UI.Toolbar.RoadEditor
{
    public class RoadEditorMenuBuilder : IMenuBuilder
    {
        public IEnumerable<IMenuCategoryBuilder> CategoryBuilders { get; private set; }

        public RoadEditorMenuBuilder()
        {
            CategoryBuilders = new[]
            {
                new RoadEditorMenuMainCategoryBuilder()
            };
        }
    }
}
