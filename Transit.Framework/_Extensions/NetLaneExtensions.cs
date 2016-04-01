using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transit.Framework
{
    public static class NetLaneExtensions
    {
        public static bool IsCreated(this NetLane lane)
        {
            return ((NetLane.Flags)lane.m_flags & NetLane.Flags.Created) != NetLane.Flags.None;
        }
    }
}
