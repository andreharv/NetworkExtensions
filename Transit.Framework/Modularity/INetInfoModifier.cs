using Transit.Framework.Interfaces;

namespace Transit.Framework.Modularity
{
    public interface INetInfoModifier : IIdentifiable
    {
        void ModifyExistingNetInfo();
    }
}
