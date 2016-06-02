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

    public enum AsymLaneType
    {
        L0R0 = 0,
        L1R2 = 12,
        L2R1 = 21,
        L1R3 = 13,
        L3R1 = 31,
        L2R3 = 23,
        L3R2 = 32,
        L2R4 = 24,
        L4R2 = 42,
        L3R5 = 35,
        L5R3 = 53
    }
}
