using System;
using ColossalFramework;
using Transit.Framework.ExtensionPoints.PathFinding.Contracts;
using Transit.Framework.ExtensionPoints.PathFinding.Implementations;
using UnityEngine;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public class TAMPathFindManager : Singleton<TAMPathFindManager>
    {
        public TAMPathFindManager()
        {
            PathFindingType = typeof (VanillaPathFind);
        }

        public Type PathFindingType { get; private set; }

        public bool IsDefaultPathFinding { get { return PathFindingType == typeof (VanillaPathFind); } }

        public void DefinePathFinding<T>()
            where T : IPathFind, new()
        {
            PathFindingType = typeof(T);
        }

        public void ResetPathFinding<T>()
            where T : IPathFind
        {
            if (PathFindingType == typeof (T))
            {
                PathFindingType = typeof(VanillaPathFind);
            }
        }    
        
        public IPathFind CreatePathFinding()
        {
            Debug.Log("TFW: Creating PathFinding of type " + PathFindingType.FullName);
            return (IPathFind)Activator.CreateInstance(PathFindingType);
        }
    }
}
