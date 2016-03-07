using System;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Vanilla;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class ExtendedPathManager
    {
        private Type _laneRoutingManagerType = typeof(VanillaLaneRoutingManager);
        private IExtendedLaneRoutingManager _laneRoutingManager = null;

        public IExtendedLaneRoutingManager LaneRoutingManager
        {
            get
            {
                if (_laneRoutingManager == null)
                {
                    _laneRoutingManager = (IExtendedLaneRoutingManager)Activator.CreateInstance(_laneRoutingManagerType);
                }

                return _laneRoutingManager;
            }
        }

        public void DefineLaneRoutingManager<T>()
            where T : IExtendedLaneRoutingManager, new()
        {
            DefineLaneRoutingManager(new T());
        }

        public void DefineLaneRoutingManager<T>(T managerInstance)
            where T : IExtendedLaneRoutingManager
        {
            _laneRoutingManagerType = typeof(T);
            _laneRoutingManager = managerInstance;
        }

        public void ResetLaneRoutingManager()
        {
            _laneRoutingManagerType = typeof(VanillaLaneRoutingManager);
            _laneRoutingManager = null;
        }

        public IExtendedLaneRoutingManager GetLaneRouting()
        {
            return _laneRoutingManager;
        }
    }
}
