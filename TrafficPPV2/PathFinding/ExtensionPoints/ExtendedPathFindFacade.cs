using Transit.Framework;
using Transit.Framework.Network;
using Transit.Framework.Redirection;

namespace CSL_Traffic
{
    public class ExtendedPathFindFacade : PathFind
    {
        private IExtendedPathFind m_innerPathFind;

        public void DefinePathFindingMethod<T>()
            where T : IExtendedPathFind, new()
        {
            m_innerPathFind = new T {Facade = this};
        }

		protected virtual void Awake()
        {
            this.m_pathfindProfiler = new ThreadProfiler();

            m_innerPathFind.OnAwake();

            typeof(PathFind)
                .GetFieldByName("m_pathFindThread")
                .SetValue(this, m_innerPathFind.PathFindThread);
		}

        protected virtual void OnDestroy()
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
