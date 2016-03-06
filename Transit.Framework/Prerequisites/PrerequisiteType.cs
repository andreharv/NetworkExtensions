using System;

namespace Transit.Framework.Prerequisites
{
    [Flags]
    public enum PrerequisiteType : ulong
    {
        None        = 0,
        UI          = 1,
        NetworkAI   = 1 << 1,
        PathFinding = 1 << 2,
    }
}
