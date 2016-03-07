using ColossalFramework;
using System;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class PathFindingManager : Singleton<PathFindingManager>
    {
        private Type _pathFindType = null;

        public void DefineCustomPathFinding<T>()
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
