using System.Collections.Generic;
using System.Threading;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.Redirection;

namespace Transit.Framework.Hooks.PathFinding
{
    public class TAMPathFind : PathFind
    {
        private static readonly Dictionary<int, IPathFindingImplementation> s_pathFinds = new Dictionary<int, IPathFindingImplementation>();

        [RedirectFrom(typeof(PathFind))]
        private void PathFindImplementation(uint unit, ref PathUnit data)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            IPathFindingImplementation pathFinder;
            if (!s_pathFinds.TryGetValue(threadId, out pathFinder))
            {
                if (PathFindingManager.instance.HasCustomPathFinding())
                {
                    pathFinder = PathFindingManager.instance.CreateCustomPathFinding();
                }
                else
                {
                    pathFinder = new TAMPathFindVanilla();
                }

                s_pathFinds.Add(threadId, pathFinder);
            }

            pathFinder.ProcessPathUnit(unit, ref data);
        }
    }
}
