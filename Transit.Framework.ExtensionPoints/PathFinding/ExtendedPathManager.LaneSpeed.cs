
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class ExtendedPathManager
    {
        private IExtendedLaneSpeedManager _laneSpeedManager = null;

        public void DefineCustomLaneSpeed<T>(T managerInstance)
            where T : IExtendedLaneSpeedManager
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

        public IExtendedLaneSpeedManager GetCustomLaneSpeed()
        {
            return _laneSpeedManager;
        }
    }
}
