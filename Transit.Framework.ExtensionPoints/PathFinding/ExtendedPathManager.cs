using System;
using ColossalFramework;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class ExtendedPathManager : Singleton<ExtendedPathManager>
    {
        public ExtendedPathFindFacade[] PathFindFacades { get; set; }

        private Type _pathFindingType = typeof(VanillaPathFind);

        public void DefinePathFinding<T>()
            where T : IExtendedPathFind, new()
        {
            _pathFindingType = typeof(T);
        }

        public void ResetPathFinding()
        {
            _pathFindingType = typeof(VanillaPathFind);
        }    
        
        public IExtendedPathFind CreatePathFinding()
        {
            return (IExtendedPathFind)Activator.CreateInstance(_pathFindingType);
        }
    }
}
