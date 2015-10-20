namespace Transit.Framework.Interfaces
{
    public interface IMenuItemConfig : ILocalizable
    {
        string UICategory { get; }
        int UIOrder { get; }

        string ThumbnailsPath { get; }
        string InfoTooltipPath { get; }
    }
}
