using System;
using System.Collections.Generic;
using Transit.Framework.Interfaces;

namespace Transit.Framework.Modularity
{
    public interface INetInfoBuilder : IActivablePart, IIdentifiable, IDisplayable, IPrefabBuilder, IMenuItemConfig, IOrderable
    {
        NetInfoVersion SupportedVersions { get; }

        void BuildUp(NetInfo info, NetInfoVersion version);
    }

    public interface IMultiNetInfoBuilder : IActivablePart, IIdentifiable, IPrefabBuilder, IOrderable
    {
        NetInfoVersionExtended SupportedVersions { get; }

        IEnumerable<NetInfo> Build();

        void BuildUp(NetInfo info, NetInfoVersionExtended version);

        IMenuItemConfig GetMenuItemConfig(NetInfoVersionExtended version);
    }

    [Flags]
    public enum NetInfoVersion
    {
        Ground = 0, //By default
        Elevated = 1,
        Bridge = 2,
        Tunnel = 4,
        Slope = 8,
        All = 15
    }

    [Flags]
    public enum NetInfoVersionExtended
    {
        Ground = 0, //By default
        Elevated = 1,
        Bridge = 2,
        Tunnel = 4,
        Slope = 8,
        GroundGrass = 16,
        GroundTrees = 32,
        All = 63,
    }
}
