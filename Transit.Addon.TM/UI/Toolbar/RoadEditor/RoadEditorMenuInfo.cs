using System.Collections.Generic;
using Transit.Framework.UI.Toolbar.Infos;

namespace Transit.Addon.TM.UI.Toolbar.RoadEditor
{
    public class RoadEditorMenuInfo : IMenuInfo
    {
        public IEnumerable<IMenuCategoryInfo> CategoryInfos { get; private set; }

        public RoadEditorMenuInfo()
        {
            CategoryInfos = new[]
            {
                new RoadEditorMenuMainCategoryInfo()
            };
        }
    }
}
