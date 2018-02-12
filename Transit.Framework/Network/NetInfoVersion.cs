using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transit.Framework
{
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
        GroundPavement = 64,
        AllWithDecoration = 63,
        AllWithDecorationAndPavement = 127
    }

    public static class NetInfoVersionExtensions
    {
        public static IEnumerable<NetInfoVersion> ToCollection(this NetInfoVersion version)
        {
            return Enum
                .GetValues(typeof (NetInfoVersion))
                .OfType<NetInfoVersion>()
                .Where(niv => niv != NetInfoVersion.All && niv != NetInfoVersion.AllWithDecoration && niv != NetInfoVersion.AllWithDecorationAndPavement)
                .Where(niv => version.HasFlag(niv));
        }
        public static bool IsGroundDecorated(this NetInfoVersion version)
        {
            return version == NetInfoVersion.GroundGrass || version == NetInfoVersion.GroundTrees || version == NetInfoVersion.GroundPavement;
        }
    }
}
