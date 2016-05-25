using System;
using ColossalFramework;
using Transit.Addon.TM.Data;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;
using System.Linq;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public partial class TAMLaneRoutingManager : Singleton<TAMLaneRoutingManager>, ILaneRoutingManager
    {
        private TAMLaneRoute[] _laneRoutes = null;

        public void Init()
        {
            _laneRoutes = new TAMLaneRoute[NetManager.MAX_LANE_COUNT];
        }

        public void Load(TAMLaneRoute[] laneRoutes)
        {
            if (laneRoutes == null)
            {
                return;
            }

            foreach (var route in laneRoutes)
            {
                Load(route);
            }
        }

        public void Load(TAMLaneRoute route)
        {
            if (route == null)
            {
                return;
            }

            if (!ScrubRoute(route))
            {
                return;
            }

            _laneRoutes[route.LaneId] = route;

            UpdateLaneArrows(route.LaneId, route.NodeId);
        }

        public void LoadLaneDirection(uint laneId, TAMLaneDirection direction)
        {
            Load(TransformToRoute(laneId, direction));
        }

        public void Reset()
        {
            _laneRoutes = null;
        }

        public bool IsLoaded()
        {
            return _laneRoutes != null;
        }

        public TAMLaneRoute[] GetAllRoutes()
        {
            return _laneRoutes;
        }

        private TAMLaneRoute CreateRoute(uint laneId)
        {
            var foundNodeId = NetManager.instance.FindLaneNodeId(laneId);
            if (foundNodeId == null)
            {
                throw new Exception(string.Format("Cannot create route, node for laneid {0} has not been found", laneId));
            }

            var laneRoute = new TAMLaneRoute()
            {
                LaneId = laneId,
                NodeId = foundNodeId.Value
            };

            _laneRoutes[laneId] = laneRoute;

            return laneRoute;
        }

        /// <summary>
        /// Gets lane data for the given lane id.
        /// The result may be null.
        /// Warning: This method is not thread-safe.
        /// </summary>
        /// <param name="laneId"></param>
        /// <returns></returns>
        public TAMLaneRoute GetRoute(uint laneId)
        {
            return _laneRoutes[laneId];
        }

        /// <summary>
        /// Gets lane data for the given lane id. Create it if does not exist.
        /// Warning: This method is not thread-safe.
        /// </summary>
        /// <param name="laneId"></param>
        /// <returns></returns>
        public TAMLaneRoute GetOrCreateRoute(uint laneId)
        {
            var lane = _laneRoutes[laneId];
            if (lane == null)
            {
                lane = CreateRoute(laneId);
            }

            return lane;
        }

        public void DestroyRoute(TAMLaneRoute route)
        {
            if (_laneRoutes == null)
            {
                return;
            }

            if (_laneRoutes[route.NodeId] == route)
            {
                _laneRoutes[route.NodeId] = null;
            }
        }

        public bool CanLanesConnect(ushort nodeId, ushort originSegmentId, byte originLaneIndex, uint originLaneId, ushort destinationSegmentId, byte destinationLaneIndex, uint destinationLaneId, ExtendedUnitType unitType)
        {
            if ((unitType & TAMSupported.UNITS) == 0)
            {
                // unit type not supported
                return true;
            }


            var originLaneInfo = NetManager.instance.GetLaneInfo(originSegmentId, originLaneIndex); // TODO query over segment id and lane index
            if (originLaneInfo == null)
            {
                // no lane info found
                return true;
            }

            if ((originLaneInfo.m_vehicleType & TAMSupported.VEHICLETYPES) == 0)
            {
                // vehicle type not supported
                return true;
            }

            var destinationLane = NetManager.instance.GetLaneInfo(destinationSegmentId, destinationLaneIndex); // TODO query over segment id and lane index
            if (destinationLane == null)
            {
                // no lane info found
                return true;
            }

            if ((destinationLane.m_vehicleType & TAMSupported.VEHICLETYPES) == 0)
            {
                // vehicle type not supported
                return true;
            }

            TAMLaneRoute route = GetRoute(originLaneId);
            if (route == null)
                return true;

            while (true)
            {
                try
                {
                    if (route.Connections.Length <= 0)
                        return true; // default

                    return route.Connections.Contains(destinationLaneId);
                }
                catch (Exception e)
                {
                    // we might get an IndexOutOfBounds here since we are not locking
#if DEBUG
						Log.Warning("ConnectsTo: " + e.ToString());
#endif
                }
            }
        }
    }
}
