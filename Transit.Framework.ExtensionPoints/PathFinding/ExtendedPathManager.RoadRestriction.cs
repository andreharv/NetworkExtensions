using System;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Implementations;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class ExtendedPathManager
    {
        private Type _roadRestrictionManagerType = typeof(VanillaRoadRestrictionManager);
        private IExtendedRoadRestrictionManager _roadRestrictionManager = null;

        public IExtendedRoadRestrictionManager RoadRestriction
        {
            get
            {
                if (_roadRestrictionManager == null)
                {
                    _roadRestrictionManager = (IExtendedRoadRestrictionManager)Activator.CreateInstance(_roadRestrictionManagerType);
                }

                return _roadRestrictionManager;
            }
        }

        public void DefineRoadRestrictionManager<T>()
            where T : IExtendedRoadRestrictionManager, new()
        {
            DefineRoadRestrictionManager(new T());
        }

        public void DefineRoadRestrictionManager<T>(T managerInstance)
            where T : IExtendedRoadRestrictionManager
        {
            _roadRestrictionManagerType = typeof(T);
            _roadRestrictionManager = managerInstance;
        }

        public void ResetRoadRestrictionManager()
        {
            _roadRestrictionManagerType = typeof(VanillaRoadRestrictionManager);
            _roadRestrictionManager = null;
        }
    }
}
