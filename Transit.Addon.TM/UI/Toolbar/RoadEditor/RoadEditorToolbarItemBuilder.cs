using System;
using System.Collections.Generic;
using Transit.Framework.Builders;

namespace Transit.Addon.TM.UI.Toolbar.RoadEditor
{
    public class RoadEditorToolbarItemBuilder : IToolbarItemBuilder
    {
        public const string NAME = "RoadEditor";

        public string Name { get { return NAME; } }
        public string DisplayName { get { return "Road Editor"; } }
        public string Description { get { return "Road Editor"; } }
        public int Order { get { return 11; } }

        public string Tutorial
        {
            get
            {
                return
                    "Intersection editor:\n\n" +
                    "1. Hover over roads and find an intersection (circle appears), then click to edit it\n" +
                    "2. Entry points will be shown, click one to select it (right-click goes back to step 1)\n" +
                    "3. Click the exit routes you wish to allow (right-click goes back to step 2)" +
                    "\n\nUse PageUp/PageDown to toggle Underground View.";
            }
        }

        public IEnumerable<IMenuCategoryBuilder> CategoryBuilders { get; private set; }

        public RoadEditorToolbarItemBuilder()
        {
            CategoryBuilders = new[]
            {
                new RoadEditorMenuMainCategoryBuilder()
            };
        }
    }
}
