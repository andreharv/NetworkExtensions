
namespace Transit.Framework.Extenders.PathFinding
{
    public partial class PathFindingProvider
    {
        private ILaneSpeedManager _laneSpeedManager = null;

        public void SetCustomLaneSpeedManager<T>(T managerInstance)
            where T : ILaneSpeedManager
        {
            _laneSpeedManager = managerInstance;
        }

        public void DisableCustomLaneSpeedManager()
        {
            _laneSpeedManager = null;
        }

        public bool HasCustomLaneSpeedManager()
        {
            return _laneSpeedManager != null;
        }

        public ILaneSpeedManager GetCustomLaneSpeedManager()
        {
            return _laneSpeedManager;
        }
    }
}
