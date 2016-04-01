using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transit.Framework
{
    public static class NetNodeExtensions
    {
        public static bool IsCreated(this NetNode node)
        {
            return (node.m_flags & NetNode.Flags.Created) != NetNode.Flags.None;
        }

        public static IEnumerable<ushort> GetSegmentIds(this NetNode node)
        {
            for (int i = 0; i < 8; i++)
            {
                var segmentId = node.GetSegment(i);
                if (segmentId != 0)
                {
                    yield return segmentId;
                }
            }
        }
    }
}
