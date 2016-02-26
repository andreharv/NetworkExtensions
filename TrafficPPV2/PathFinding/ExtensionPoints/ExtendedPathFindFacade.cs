using System;
using Transit.Framework;
using Transit.Framework.Network;
using Transit.Framework.Redirection;

namespace CSL_Traffic
{
    public class ExtendedPathFindFacade : PathFind
    {
        private static IExtendedPathFind m_innerPathFind;

        public ExtendedPathFindFacade()
        {
            m_innerPathFind = ExtendedPathManager.CreatePathFinding();
        }

		protected void Awake()
        {
            this.m_pathfindProfiler = new ThreadProfiler();

            m_innerPathFind.OnAwake();

            typeof(PathFind)
                .GetFieldByName("m_pathFindThread")
                .SetValue(this, m_innerPathFind.PathFindThread);
		}

        protected void OnDestroy()
        {
            m_innerPathFind.OnDestroy();
		}

        [RedirectFrom(typeof(PathFind))]
        public new bool CalculatePath(uint unit, bool skipQueue)
        {
            return m_innerPathFind.CalculatePath(unit, skipQueue, ExtendedVehicleType.Unknown);
        }

        public bool CalculatePath(uint unit, bool skipQueue, ExtendedVehicleType vehicleType)
        {
            return m_innerPathFind.CalculatePath(unit, skipQueue, vehicleType);
        }

        [RedirectFrom(typeof(PathFind))]
        public new void WaitForAllPaths()
        {
            m_innerPathFind.WaitForAllPaths();
        }
	}   
}
