namespace Transit.Framework.Interfaces
{
    public interface IMenuItemBuilder : ILocalizable
    {
        string UICategory { get; }
        int UIOrder { get; }

        string ThumbnailsPath { get; }
        string InfoTooltipPath { get; }
    }
}
