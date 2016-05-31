using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public class ExtendedPathFindFacade : PathFind
    {
        private readonly IExtendedPathFind m_innerPathFind;

        public ExtendedPathFindFacade()
        {
            m_innerPathFind = this.CreatePathFinding();
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

        public bool CalculatePath(uint unit, bool skipQueue, ExtendedVehicleType vehicleType)
        {
            return m_innerPathFind.CalculatePath(unit, skipQueue, vehicleType);
        }
        
        public new void WaitForAllPaths()
        {
            m_innerPathFind.WaitForAllPaths();
        }
	}   
}
