using System;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Implementations;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class ExtendedPathManager
    {
        private Type _laneRoutingManagerType = typeof(VanillaLaneRoutingManager);
        private ILaneRoutingManager _laneRoutingManager = null;

        public ILaneRoutingManager LaneRouting
        {
            get
            {
                if (_laneRoutingManager == null)
                {
                    _laneRoutingManager = (ILaneRoutingManager)Activator.CreateInstance(_laneRoutingManagerType);
                }

                return _laneRoutingManager;
            }
        }

        public void DefineLaneRoutingManager<T>()
            where T : ILaneRoutingManager, new()
        {
            DefineLaneRoutingManager(new T());
        }

        public void DefineLaneRoutingManager<T>(T managerInstance)
            where T : ILaneRoutingManager
        {
            _laneRoutingManagerType = typeof(T);
            _laneRoutingManager = managerInstance;
        }

        public void ResetLaneRoutingManager<T>()
            where T : ILaneRoutingManager
        {
            if (_laneRoutingManagerType == typeof(T))
            {
                _laneRoutingManagerType = typeof(VanillaLaneRoutingManager);
                _laneRoutingManager = null;
            }
        }
    }
}
