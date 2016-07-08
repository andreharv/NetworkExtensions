using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public enum CenterLaneType
    {
        None = 0,
        TurningLane = 1,
        Median = 2
    }

    public enum LanesLayoutStyle
    {
        Symetrical = 0,
        AsymL1R2 = 12,
        AsymL1R3 = 13,
        AsymL3R1 = 31,
        AsymL2R3 = 23,
        AsymL3R2 = 32,
        AsymL2R4 = 24,
        AsymL4R2 = 42,
        AsymL3R5 = 35,
        AsymL5R3 = 53
    }
}
