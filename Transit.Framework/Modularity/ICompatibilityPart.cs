using System.Collections.Generic;
using Transit.Framework.Interfaces;

namespace Transit.Framework.Modularity
{
    public interface ICompatibilityPart : IModulePart, IIdentifiable
    {
        bool IsPluginActive { get; }

        void Setup(IEnumerable<NetInfo> newRoads);
    }
}
