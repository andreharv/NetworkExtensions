using Transit.Framework.Interfaces;

namespace Transit.Framework.Builders
{
    public interface IMenuItemBuilder : ILocalizable
    {
        string UICategory { get; }
        int UIOrder { get; }

        string ThumbnailsPath { get; }
        string InfoTooltipPath { get; }
    }
}
