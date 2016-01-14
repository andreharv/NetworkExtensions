using System.Collections.Generic;
using System.Threading;
using Transit.Framework.Unsafe;

namespace Transit.Addon.TrafficPP.PathFinding
{
    public class TAMPathFind : PathFind
    {
        private static readonly Dictionary<int, TAMPathFindImplementation> s_pathFinds = new Dictionary<int, TAMPathFindImplementation>();

        [RedirectFrom(typeof(PathFind))]
        private void PathFindImplementation(uint unit, ref PathUnit data)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            TAMPathFindImplementation pathFindImplementation;
            if (!s_pathFinds.TryGetValue(threadId, out pathFindImplementation))
            {
                pathFindImplementation = new TAMPathFindImplementation();
                s_pathFinds.Add(threadId, pathFindImplementation);
            }

            pathFindImplementation.PathFindImplementation(unit, ref data);
        }
    }
}
