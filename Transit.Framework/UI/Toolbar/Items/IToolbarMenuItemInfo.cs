using System;

namespace Transit.Framework.UI.Toolbar.Items
{
    public interface IToolbarMenuItemInfo : IToolbarItemInfo
    {
        string Name { get; }
        string Description { get; }
        string ThumbnailsPath { get; }
        Type PanelType { get; }
    }
}
