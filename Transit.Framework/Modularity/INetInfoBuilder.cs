using System;

namespace Transit.Framework.Modularity
{
    public interface INetInfoBuilder : IActivablePart
    {
        int Priority { get; }

        string TemplatePrefabName { get; }
        string CodeName { get; }
        string Description { get; }

        string UICategory { get; }
        string ThumbnailsPath { get; }
        string InfoTooltipPath { get; }

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
        All = 15
    }
}
