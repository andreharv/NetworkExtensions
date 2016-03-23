using Transit.Framework.Interfaces;

namespace Transit.Framework.Builders
{
    public interface IToolbarItemBuilder : IIdentifiable, IOrderable, IDescriptor
    {
        IMenuBuilder MenuBuilder { get; }
    }
}
