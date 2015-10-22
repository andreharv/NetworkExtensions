using Transit.Framework.Interfaces;

namespace Transit.Framework
{
    public class MenuItemBuilder : IMenuItemBuilder
    {
        public string UICategory { get; set; }

        public int UIOrder { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string ThumbnailsPath { get; set; }

        public string InfoTooltipPath { get; set; }
    }
}
