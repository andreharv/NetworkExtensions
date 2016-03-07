using System;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Vanilla;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class ExtendedPathManager
    {
        private Type _laneSpeedManagerType = typeof(VanillaLaneSpeedManager);
        private IExtendedLaneSpeedManager _laneSpeedManager = null;

        public IExtendedLaneSpeedManager LaneSpeedManager
        {
            get
            {
                if (_laneSpeedManager == null)
                {
                    _laneSpeedManager = (IExtendedLaneSpeedManager) Activator.CreateInstance(_laneSpeedManagerType);
                }

                return _laneSpeedManager;
            } 
        }

        public void DefineLaneSpeedManager<T>()
            where T : IExtendedLaneSpeedManager, new()
        {
            DefineLaneSpeedManager(new T());
        }

        public void DefineLaneSpeedManager<T>(T managerInstance)
            where T : IExtendedLaneSpeedManager
        {
            _laneSpeedManagerType = typeof (T);
            _laneSpeedManager = managerInstance;
        }

        public void ResetLaneSpeedManager()
        {
            _laneSpeedManagerType = typeof(VanillaLaneSpeedManager);
            _laneSpeedManager = null;
        }

        public IExtendedLaneSpeedManager GetLaneSpeed()
        {
            return _laneSpeedManager;
        }
    }
}
