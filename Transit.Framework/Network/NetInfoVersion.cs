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
        AllWithDecoration = 63,
    }
    public enum SpecailSegments
    {
        EndNode = 0,
        BusStation = 2,
        None = 4
    }
    public static class NetInfoVersionExtensions
    {
        public static IEnumerable<NetInfoVersion> ToCollection(this NetInfoVersion version)
        {
            return Enum
                .GetValues(typeof (NetInfoVersion))
                .OfType<NetInfoVersion>()
                .Where(niv => niv != NetInfoVersion.All && niv != NetInfoVersion.AllWithDecoration)
                .Where(niv => version.HasFlag(niv));
        }
    }
}
