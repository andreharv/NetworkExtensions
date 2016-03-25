using System;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Implementations;

namespace Transit.Framework.ExtensionPoints.PathFindingFeatures
{
    public partial class TAMPathFindFeatureManager
    {
        private Type _restrictionManagerType = typeof(VanillaRestrictionManager);
        private IRestrictionManager _restrictionManager = null;

        public IRestrictionManager Restrictions
        {
            get
            {
                if (_restrictionManager == null)
                {
                    _restrictionManager = (IRestrictionManager)Activator.CreateInstance(_restrictionManagerType);
                }

                return _restrictionManager;
            }
        }

        public void DefineRestrictionManager<T>()
            where T : IRestrictionManager, new()
        {
			DefineRestrictionManager(new T());
        }

        public void DefineRestrictionManager<T>(T managerInstance)
            where T : IRestrictionManager
        {
			UnityEngine.Debug.Log($"New RestrictionManager: {managerInstance.ToString()}");
			_restrictionManagerType = typeof(T);
            _restrictionManager = managerInstance;
        }

        public void ResetRestrictionManager<T>()
            where T : IRestrictionManager
        {
            if (_restrictionManagerType == typeof(T))
            {
                _restrictionManagerType = typeof(VanillaRestrictionManager);
                _restrictionManager = null;
            }
        }
    }
}
