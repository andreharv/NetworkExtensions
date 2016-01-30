using ColossalFramework;
using System;

namespace Transit.Framework.Extenders.PathFinding
{
    public partial class PathFindingProvider : Singleton<PathFindingProvider>
    {
        private Type _pathFindType = null;

        public void SetCustomPathFinding<T>()
            where T : IPathFindingImplementation, new()
        {
            _pathFindType = typeof(T);
        }

        public void DisableCustomPathFinding()
        {
            _pathFindType = null;
        }

        public bool HasCustomPathFinding()
        {
            return _pathFindType != null;
        }

        public IPathFindingImplementation CreateCustomPathFinding()
        {
            return (IPathFindingImplementation)Activator.CreateInstance(_pathFindType);
        }
    }
}
