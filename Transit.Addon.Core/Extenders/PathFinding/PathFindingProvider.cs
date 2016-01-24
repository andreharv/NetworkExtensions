using System;
using ColossalFramework;
using Transit.Addon.Core.Prerequisites.PathFinding;

namespace Transit.Addon.Core.Extenders.PathFinding
{
    public class PathFindingProvider : Singleton<PathFindingProvider>
    {
        private Type _pathFindType = typeof(VanillaPathFinding);

        public void SetType<T>()
            where T : IPathFindingImplementation, new()
        {
            _pathFindType = typeof(T);
        }

        public void ResetToDefault()
        {
            _pathFindType = typeof(VanillaPathFinding);
        }

        public IPathFindingImplementation CreatePathFinding()
        {
            return (IPathFindingImplementation)Activator.CreateInstance(_pathFindType);
        }
    }
}
