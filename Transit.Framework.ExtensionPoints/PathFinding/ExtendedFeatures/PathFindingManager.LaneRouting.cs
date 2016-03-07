
namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class PathFindingManager
    {
        private ILaneRoutingManager _laneRoutingManager = null;

        public void DefineCustomLaneRouting<T>(T managerInstance)
            where T : ILaneRoutingManager
        {
            _laneRoutingManager = managerInstance;
        }

        public void DisableCustomLaneRouting()
        {
            _laneRoutingManager = null;
        }

        public bool HasCustomLaneRouting()
        {
            return _laneRoutingManager != null;
        }

        public ILaneRoutingManager GetCustomLaneRouting()
        {
            return _laneRoutingManager;
        }
    }
}
