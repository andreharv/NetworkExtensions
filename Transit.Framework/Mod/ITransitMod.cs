using ICities;
using Transit.Framework.Prerequisites;

namespace Transit.Framework.Mod
{
    public interface ITransitMod : IUserMod
    {
        PrerequisiteType Requirements { get; }
    }
}
