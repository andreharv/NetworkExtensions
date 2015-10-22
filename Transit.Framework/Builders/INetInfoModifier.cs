using Transit.Framework.Interfaces;

namespace Transit.Framework.Builders
{
    public interface INetInfoModifier : IIdentifiable
    {
        void ModifyExistingNetInfo();
    }
}
