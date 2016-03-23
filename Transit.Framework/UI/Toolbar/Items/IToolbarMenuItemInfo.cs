using System;
using Transit.Framework.Interfaces;

namespace Transit.Framework.UI.Toolbar.Items
{
    public interface IToolbarMenuItemInfo : IIdentifiable, IOrderable
    {
        Type PanelType { get; }
    }
}
