using System;

namespace Transit.Framework.Prerequisites
{
    [Flags]
    public enum PrerequisiteType : ulong
    {
        None        = 0,
        UI          = 1,
        AI          = 1 << 1,
        Pathfinding = 1 << 2,
    }
}
