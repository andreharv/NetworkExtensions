using System;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.Network;
using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;

namespace Transit.Framework.Hooks.PathFinding
{
    public class PathFindHook : PathFind
    {
        [RedirectFrom(typeof (PathFind), (ulong) PrerequisiteType.PathFinding)]
        private void Awake()
        {
            // Disabling the vanilla Awake method
        }

        [RedirectFrom(typeof (PathFind), (ulong) PrerequisiteType.PathFinding)]
        private void OnDestroy()
        {
            // Disabling the vanilla Awake method
        }

        [RedirectFrom(typeof (PathFind), (ulong) PrerequisiteType.PathFinding)]
        public new bool CalculatePath(uint unit, bool skipQueue)
        {
            ExtendedPathFindFacade facade;
            
            // ReSharper disable SuspiciousTypeConversion.Global
            if ((object)this is ExtendedPathFindFacade)
            // ReSharper restore SuspiciousTypeConversion.Global
            // ReSharper disable HeuristicUnreachableCode
            {
                // ReSharper disable SuspiciousTypeConversion.Global
                facade = (ExtendedPathFindFacade)(object)this;
                // ReSharper restore SuspiciousTypeConversion.Global
            }
            // ReSharper restore HeuristicUnreachableCode
            else
            {
                throw new Exception("ExtendedPathManager is not installed correctly");
            }

            return facade.CalculatePath(unit, skipQueue, ExtendedVehicleType.Unknown);
        }

        [RedirectFrom(typeof(PathFind), (ulong)PrerequisiteType.PathFinding)]
        public new void WaitForAllPaths()
        {
            ExtendedPathFindFacade facade;
            
            // ReSharper disable SuspiciousTypeConversion.Global
            if ((object)this is ExtendedPathFindFacade)
            // ReSharper restore SuspiciousTypeConversion.Global
            // ReSharper disable HeuristicUnreachableCode
            {
                // ReSharper disable SuspiciousTypeConversion.Global
                facade = (ExtendedPathFindFacade)(object)this;
                // ReSharper restore SuspiciousTypeConversion.Global
            }
            // ReSharper restore HeuristicUnreachableCode
            else
            {
                throw new Exception("ExtendedPathManager is not installed correctly");
            }

            // ReSharper disable HeuristicUnreachableCode
            facade.WaitForAllPaths();
            // ReSharper restore HeuristicUnreachableCode
        }
    }
}
