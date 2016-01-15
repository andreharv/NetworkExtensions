
using System;

namespace Transit.Addon.Core.PathFinding
{
    public static class PathFinder
    {
        private static Type s_pathFindType = typeof(DefaultPathFinder);

        public static void SetType<T>()
            where T : IPathFinder, new()
        {
            s_pathFindType = typeof(T);
        }

        public static void ResetToDefault()
        {
            s_pathFindType = typeof(DefaultPathFinder);
        }

        public static IPathFinder CreateInstance()
        {
            return (IPathFinder)Activator.CreateInstance(s_pathFindType);
        }
    }
}
