using System;
using Transit.Framework.Interfaces;

namespace Transit.Framework.Modularity
{
    public interface INetInfoBuilderPart : INetInfoBuilder, IActivablePart, IIdentifiable, IDisplayable, IMenuItemBuilder, IOrderable
    {
    }

    public interface IMultiNetInfoBuilderPart : INetInfoBuilder, IActivablePart, IIdentifiable, IDisplayable, IMenuItemBuildersProvider, IOrderable
    {
    }

    public interface INetInfoBuilder : IPrefabBuilder, IIdentifiable
    {
        NetInfoVersion SupportedVersions { get; }

        void BuildUp(NetInfo info, NetInfoVersion version);
    }

    [Flags]
    public enum NetInfoVersion
    {
        Ground = 0, //By default
        Elevated = 1,
        Bridge = 2,
        Tunnel = 4,
        Slope = 8,
        All = 15,
        GroundGrass = 16,
        GroundTrees = 32,
        AllWithDecoration = 63,
    }
}
