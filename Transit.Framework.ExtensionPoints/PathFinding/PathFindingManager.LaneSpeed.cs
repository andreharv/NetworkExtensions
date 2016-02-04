
namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class PathFindingManager
    {
        private ILaneSpeedManager _laneSpeedManager = null;

        public void DefineCustomLaneSpeed<T>(T managerInstance)
            where T : ILaneSpeedManager
        {
            _laneSpeedManager = managerInstance;
        }

        public void DisableCustomLaneSpeed()
        {
            _laneSpeedManager = null;
        }

        public bool HasCustomLaneSpeed()
        {
            return _laneSpeedManager != null;
        }

        public ILaneSpeedManager GetCustomLaneSpeed()
        {
            return _laneSpeedManager;
        }
    }
}
