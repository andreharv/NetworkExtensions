using System;
using ICities;

namespace Transit.Framework.Modularity
{
    public interface ITransitMod : IUserMod
    {
        UInt64 WorkshopId { get; }
    }
}
