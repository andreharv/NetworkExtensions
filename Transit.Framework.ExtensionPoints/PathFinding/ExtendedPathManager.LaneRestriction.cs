using System;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Vanilla;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class ExtendedPathManager
    {
        private Type _laneRestrictionManagerType = typeof(VanillaLaneRestrictionManager);
        private IExtendedLaneRestrictionManager _laneRestrictionManager = null;

        public IExtendedLaneRestrictionManager LaneRestrictionManager
        {
            get
            {
                if (_laneRestrictionManager == null)
                {
                    _laneRestrictionManager = (IExtendedLaneRestrictionManager)Activator.CreateInstance(_laneRestrictionManagerType);
                }

                return _laneRestrictionManager;
            }
        }

        public void DefineLaneRestrictionManager<T>()
            where T : IExtendedLaneRestrictionManager, new()
        {
            DefineLaneRestrictionManager(new T());
        }

        public void DefineLaneRestrictionManager<T>(T managerInstance)
            where T : IExtendedLaneRestrictionManager
        {
            _laneRestrictionManagerType = typeof(T);
            _laneRestrictionManager = managerInstance;
        }

        public void ResetLaneRestrictionManager()
        {
            _laneRestrictionManagerType = typeof(VanillaLaneRestrictionManager);
            _laneRestrictionManager = null;
        }

        public IExtendedLaneRestrictionManager GetLaneRestriction()
        {
            return _laneRestrictionManager;
        }
    }
}
