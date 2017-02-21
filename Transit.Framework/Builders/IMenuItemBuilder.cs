using Transit.Framework.Interfaces;

namespace Transit.Framework.Builders
{
    public interface IMenuItemBuilder : IIdentifiable, IDisplayable, IDescriptor
    {
        string UICategory { get; }
        int UIOrder { get; }
        string ThumbnailsPath { get; }
        string InfoTooltipPath { get; }
    }
}
