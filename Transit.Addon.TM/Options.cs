using System;

namespace Transit.Addon.TM
{
    [Flags]
    public enum Options : long
    {
        None = 0,
        UseRealisticSpeeds = 8,
        NoDespawn = 16,
        RoadCustomizerTool = 1L << 55,
    }
}