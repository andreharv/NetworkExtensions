
namespace Transit.Addon.Core.Extenders.PathFinding
{
    public partial class PathFindingProvider
    {
        private ILaneRoutingManager _laneRoutingManager = null;

        public void SetCustomLaneRoutingManager<T>(T managerInstance)
            where T : ILaneRoutingManager
        {
            _laneRoutingManager = managerInstance;
        }

        public void DisableCustomLaneRoutingManager()
        {
            _laneRoutingManager = null;
        }

        public bool HasCustomLaneRoutingManager()
        {
            return _laneRoutingManager != null;
        }

        public ILaneRoutingManager GetCustomLaneRoutingManager()
        {
            return _laneRoutingManager;
        }
    }
}
