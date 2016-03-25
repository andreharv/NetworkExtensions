using Transit.Framework.Interfaces;

namespace Transit.Framework.UI.Infos
{
    public interface IMenuCategoryInfo : IIdentifiable, IDisplayable, IOrderable
    {
        GeneratedGroupPanel.GroupFilter? Group { get; }
        ItemClass.Service? Service { get; }
    }
}
