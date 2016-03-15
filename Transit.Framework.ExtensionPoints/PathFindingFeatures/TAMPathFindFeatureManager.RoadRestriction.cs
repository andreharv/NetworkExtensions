using System;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Implementations;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures
{
    public partial class TAMPathFindFeatureManager
    {
        private Type _roadRestrictionManagerType = typeof(VanillaRoadRestrictionManager);
        private IRoadRestrictionManager _roadRestrictionManager = null;

        public IRoadRestrictionManager RoadRestriction
        {
            get
            {
                if (_roadRestrictionManager == null)
                {
                    _roadRestrictionManager = (IRoadRestrictionManager)Activator.CreateInstance(_roadRestrictionManagerType);
                }

                return _roadRestrictionManager;
            }
        }

        public void DefineRoadRestrictionManager<T>()
            where T : IRoadRestrictionManager, new()
        {
			DefineRoadRestrictionManager(new T());
        }

        public void DefineRoadRestrictionManager<T>(T managerInstance)
            where T : IRoadRestrictionManager
        {
			UnityEngine.Debug.Log($"New RoadRestrictionManager: {managerInstance.ToString()}");
			_roadRestrictionManagerType = typeof(T);
            _roadRestrictionManager = managerInstance;
        }

        public void ResetRoadRestrictionManager<T>()
            where T : IRoadRestrictionManager
        {
            if (_roadRestrictionManagerType == typeof(T))
            {
                _roadRestrictionManagerType = typeof(VanillaRoadRestrictionManager);
                _roadRestrictionManager = null;
            }
        }
    }
}
