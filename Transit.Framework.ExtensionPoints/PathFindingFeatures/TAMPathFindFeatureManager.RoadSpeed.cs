using System;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Implementations;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures
{
    public partial class TAMPathFindFeatureManager
    {
        private Type _roadSpeedManagerType = typeof(VanillaRoadSpeedManager);
        private IRoadSpeedManager _roadSpeedManager = null;

        public IRoadSpeedManager RoadSpeed
        {
            get
            {
                if (_roadSpeedManager == null)
                {
                    _roadSpeedManager = (IRoadSpeedManager) Activator.CreateInstance(_roadSpeedManagerType);
                }

                return _roadSpeedManager;
            } 
        }

        public void DefineRoadSpeedManager<T>()
            where T : IRoadSpeedManager, new()
        {
            DefineRoadSpeedManager(new T());
        }

        public void DefineRoadSpeedManager<T>(T managerInstance)
            where T : IRoadSpeedManager
        {
            _roadSpeedManagerType = typeof (T);
            _roadSpeedManager = managerInstance;
        }

        public void ResetRoadSpeedManager<T>()
            where T : IRoadSpeedManager
        {
            if (_roadSpeedManagerType == typeof(T))
            {
                _roadSpeedManagerType = typeof(VanillaRoadSpeedManager);
                _roadSpeedManager = null;
            }
        }
    }
}
