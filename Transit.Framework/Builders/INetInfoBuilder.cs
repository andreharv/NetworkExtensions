using System;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;

namespace Transit.Framework.Builders
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
}
