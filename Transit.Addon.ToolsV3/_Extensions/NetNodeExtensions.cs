using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transit.Addon.ToolsV3
{
    public static class NetNodeExtensions
    {
        public static IEnumerable<ushort> GetSegments(this NetNode node)
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
