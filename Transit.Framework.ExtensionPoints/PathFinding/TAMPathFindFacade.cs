using Transit.Framework.ExtensionPoints.PathFinding.Contracts;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public class TAMPathFindFacade : PathFind
    {
        private readonly IPathFind m_innerPathFind;

        public TAMPathFindFacade()
        {
            m_innerPathFind = TAMPathFindManager.instance.CreatePathFinding();
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

        public bool CalculatePath(ExtendedUnitType unitType, uint unit, bool skipQueue)
        {
            return m_innerPathFind.CalculatePath(unitType, unit, skipQueue);
        }
        
        public new void WaitForAllPaths()
        {
            m_innerPathFind.WaitForAllPaths();
        }
	}   
}
