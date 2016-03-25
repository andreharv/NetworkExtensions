using System;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Implementations;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures
{
    public partial class TAMPathFindFeatureManager
    {
        private Type _speedLimitManagerType = typeof(VanillaSpeedLimitManager);
        private ISpeedLimitManager _speedLimitManager = null;

        public ISpeedLimitManager SpeedLimits
        {
            get
            {
                if (_speedLimitManager == null)
                {
                    _speedLimitManager = (ISpeedLimitManager) Activator.CreateInstance(_speedLimitManagerType);
                }

                return _speedLimitManager;
            } 
        }

        public void DefineSpeedLimitManager<T>()
            where T : ISpeedLimitManager, new()
        {
            DefineSpeedLimitManager(new T());
        }

        public void DefineSpeedLimitManager<T>(T managerInstance)
            where T : ISpeedLimitManager
        {
			UnityEngine.Debug.Log($"New SpeedLimitManager: {managerInstance.ToString()}");
			_speedLimitManagerType = typeof (T);
            _speedLimitManager = managerInstance;
        }

        public void ResetSpeedLimitManager<T>()
            where T : ISpeedLimitManager
        {
            if (_speedLimitManagerType == typeof(T))
            {
                _speedLimitManagerType = typeof(VanillaSpeedLimitManager);
                _speedLimitManager = null;
            }
        }
    }
}
