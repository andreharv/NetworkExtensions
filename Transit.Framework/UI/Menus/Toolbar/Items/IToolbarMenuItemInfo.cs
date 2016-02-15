using System;
using Transit.Framework.Interfaces;

namespace Transit.Framework.UI.Menus.Toolbar.Items
{
    public interface IToolbarMenuItemInfo : IToolbarItemInfo, IIdentifiable
    {
        Type PanelType { get; }
    }
}
