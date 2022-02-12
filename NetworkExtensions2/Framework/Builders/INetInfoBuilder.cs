using System;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;

namespace Transit.Framework.Builders
{
    public interface INetInfoBuilderPart : INetInfoBuilder, IActivablePart, IIdentifiable, IDisplayable, IMenuItemBuilder, IOrderable, IShortDescriptor
    {
    }

    public interface IMultiNetInfoBuilderPart : INetInfoBuilder, IActivablePart, IIdentifiable, IDisplayable, IMenuItemBuildersProvider, IOrderable, IShortDescriptor
    {
    }

    public interface INetInfoBuilder : IPrefabBuilder, IIdentifiable
    {
        NetInfoVersion SupportedVersions { get; }

        void BuildUp(NetInfo info, NetInfoVersion version);
    }

    public interface INetInfoSpecificBaseBuilder : INetInfoBuilder
    {
        string GetSpecificBasedPrefabName(NetInfoVersion version);
    }

    public interface INetInfoSpecificNameBuilder : INetInfoBuilder
    {
        string GetSpecificBuiltPrefabName(NetInfoVersion version);
    }
}
