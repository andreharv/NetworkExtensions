using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transit.Framework
{
    public static class NetSegmentExtensions
    {
        public static bool IsCreated(this NetSegment segment)
        {
            return (segment.m_flags & NetSegment.Flags.Created) != NetSegment.Flags.None;
        }
    }
}
