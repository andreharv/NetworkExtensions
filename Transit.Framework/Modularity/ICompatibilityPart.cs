using System.Collections.Generic;

namespace Transit.Framework.Modularity
{
    public interface ICompatibilityPart : IModulePart
    {
        bool IsPluginActive { get; }

        void Setup(IEnumerable<NetInfo> newRoads);
    }
}
