using System;
using ColossalFramework;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Implementations;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class ExtendedPathManager : Singleton<ExtendedPathManager>
    {
        public ExtendedPathFindFacade[] PathFindFacades { get; set; }

        private Type _pathFindingType = typeof(VanillaPathFind);

        public void DefinePathFinding<T>()
            where T : IPathFind, new()
        {
            _pathFindingType = typeof(T);
        }

        public void ResetPathFinding<T>()
            where T : IPathFind
        {
            if (_pathFindingType == typeof (T))
            {
                _pathFindingType = typeof(VanillaPathFind);
            }
        }    
        
        public IPathFind CreatePathFinding()
        {
            return (IPathFind)Activator.CreateInstance(_pathFindingType);
        }
    }
}
