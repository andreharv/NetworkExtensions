using System;
using ICities;

namespace Transit.Framework.Modularity
{
    public interface ITransitMod : IUserMod
    {
        string DefaultFolderPath { get; }

        UInt64 WorkshopId { get; }
    }
}
