
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public partial class ExtendedPathManager
    {
        private IExtendedLaneRoutingManager _laneRoutingManager = null;

        public void DefineCustomLaneRouting<T>(T managerInstance)
            where T : IExtendedLaneRoutingManager
        {
            _laneRoutingManager = managerInstance;
        }

        public void DisableCustomLaneRouting()
        {
            _laneRoutingManager = null;
        }

        public bool HasCustomLaneRouting()
        {
            return _laneRoutingManager != null;
        }

        public IExtendedLaneRoutingManager GetCustomLaneRouting()
        {
            return _laneRoutingManager;
        }
    }
}
