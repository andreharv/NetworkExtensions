using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public class ExtendedPathFindFacade : PathFind
    {
        private readonly IPathFind m_innerPathFind;

        public ExtendedPathFindFacade()
        {
            m_innerPathFind = ExtendedPathManager.instance.CreatePathFinding();
            m_innerPathFind.Facade = this;
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

        public bool CalculatePath(ExtendedVehicleType vehicleType, uint unit, bool skipQueue)
        {
            return m_innerPathFind.CalculatePath(vehicleType, unit, skipQueue);
        }
        
        public new void WaitForAllPaths()
        {
            m_innerPathFind.WaitForAllPaths();
        }
	}   
}
