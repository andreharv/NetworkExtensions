using System;
using ColossalFramework;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Implementations;
using UnityEngine;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class ExtendedPathManager : Singleton<ExtendedPathManager>
    {
        public ExtendedPathFindFacade[] PathFindFacades { get; set; }

        private Type _pathFindingType = typeof(VanillaPathFind);

        public void DefinePathFinding<T>(bool onlyIfVanilla = false)
            where T : IPathFind, new()
        {
            if (onlyIfVanilla)
            {
                if (_pathFindingType != typeof (VanillaPathFind))
                {
                    return;
                }
            }

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
            Debug.Log("TFW: Creating PathFinding of type " + _pathFindingType.FullName);
            return (IPathFind)Activator.CreateInstance(_pathFindingType);
        }
    }
}
