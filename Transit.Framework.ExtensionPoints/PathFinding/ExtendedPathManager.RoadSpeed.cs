using System;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Implementations;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class ExtendedPathManager
    {
        private Type _roadSpeedManagerType = typeof(VanillaRoadSpeedManager);
        private IExtendedRoadSpeedManager _roadSpeedManager = null;

        public IExtendedRoadSpeedManager RoadSpeed
        {
            get
            {
                if (_roadSpeedManager == null)
                {
                    _roadSpeedManager = (IExtendedRoadSpeedManager) Activator.CreateInstance(_roadSpeedManagerType);
                }

                return _roadSpeedManager;
            } 
        }

        public void DefineRoadSpeedManager<T>()
            where T : IExtendedRoadSpeedManager, new()
        {
            DefineRoadSpeedManager(new T());
        }

        public void DefineRoadSpeedManager<T>(T managerInstance)
            where T : IExtendedRoadSpeedManager
        {
            _roadSpeedManagerType = typeof (T);
            _roadSpeedManager = managerInstance;
        }

        public void ResetRoadSpeedManager()
        {
            _roadSpeedManagerType = typeof(VanillaRoadSpeedManager);
            _roadSpeedManager = null;
        }
    }
}
