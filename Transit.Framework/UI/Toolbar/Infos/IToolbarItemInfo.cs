using Transit.Framework.Interfaces;

namespace Transit.Framework.UI.Toolbar.Infos
{
    public interface IToolbarItemInfo : IIdentifiable, IOrderable, IDescriptor
    {
        IMenuInfo MenuInfo { get; }
    }
}
