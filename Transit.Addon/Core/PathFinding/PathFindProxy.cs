using System;
using System.Collections.Generic;
using System.Threading;
using Transit.Framework.Unsafe;

namespace Transit.Addon.Core.PathFinding
{
    public class PathFindProxy : PathFind
    {
        private static readonly Dictionary<int, IPathFinder> s_pathFinds = new Dictionary<int, IPathFinder>();

        [RedirectFrom(typeof(PathFind))]
        private void PathFindImplementation(uint unit, ref PathUnit data)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            IPathFinder pathFinder;
            if (!s_pathFinds.TryGetValue(threadId, out pathFinder))
            {
                pathFinder = PathFinder.CreateInstance();
                s_pathFinds.Add(threadId, pathFinder);
            }

            pathFinder.ProcessPathUnit(unit, ref data);
        }
    }
}
