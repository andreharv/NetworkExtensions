using System;
using ColossalFramework;
using Transit.Addon.TM.Data;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public class TPPLaneRoutingManager : Singleton<TPPLaneRoutingManager>, ILaneRoutingManager
    {
        private TAMLaneRoute[] _laneRoutes = null;
        private readonly uint[] NO_CONNECTIONS = new uint[0];

        public void Init(TAMLaneRoute[] laneRoutes)
        {
            _laneRoutes = laneRoutes;
            if (_laneRoutes == null)
            {
                _laneRoutes = new TAMLaneRoute[NetManager.MAX_LANE_COUNT];
            }

            foreach (TAMLaneRoute laneRoute in _laneRoutes)
            {
                if (laneRoute == null)
                    continue;

                laneRoute.UpdateArrows();
            }
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

        private TAMLaneRoute CreateLaneRoute(uint laneId)
        {
            var laneRoute = new TAMLaneRoute()
            {
                LaneId = laneId
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
            TAMLaneRoute lane = _laneRoutes[laneId];
            if (lane == null)
                lane = CreateLaneRoute(laneId);

            return lane;
        }

        public bool AddLaneConnection(uint fromLaneId, uint toLaneId)
        {
            TAMLaneRoute lane = GetOrCreateRoute(fromLaneId);
            GetOrCreateRoute(toLaneId); // makes sure lane information is stored

            return lane.AddConnection(toLaneId);
        }

        public bool RemoveLaneConnection(uint laneId, uint connectionId)
        {
            TAMLaneRoute lane = GetRoute(laneId);
            if (lane == null)
                return false;

            return lane.RemoveConnection(connectionId);
        }

        public uint[] GetLaneConnections(uint laneId)
        {
            TAMLaneRoute lane = GetRoute(laneId);

            if (lane == null)
                return NO_CONNECTIONS;
            return lane.Connections;
        }

        public bool CanLanesConnect(ushort nodeId, ushort originSegmentId, byte originLaneIndex, uint originLaneId, ushort destinationSegmentId, byte destinationLaneIndex, uint destinationLaneId, ExtendedUnitType unitType) {
			if ((unitType & TPPSupported.UNITS) == 0) {
				// unit type not supported
				return true;
			}


			var originLaneInfo = NetManager.instance.GetLaneInfo(originSegmentId, originLaneIndex); // TODO query over segment id and lane index
			if (originLaneInfo == null) {
				// no lane info found
				return true;
			}

			if ((originLaneInfo.m_vehicleType & TPPSupported.VEHICLETYPES) == 0) {
				// vehicle type not supported
				return true;
			}

			var destinationLane = NetManager.instance.GetLaneInfo(destinationSegmentId, destinationLaneIndex); // TODO query over segment id and lane index
			if (destinationLane == null) {
				// no lane info found
				return true;
			}

			if ((destinationLane.m_vehicleType & TPPSupported.VEHICLETYPES) == 0) {
				// vehicle type not supported
				return true;
			}

			TAMLaneRoute route = GetRoute(originLaneId);
			if (route == null)
				return true;

			return route.ConnectsTo(destinationLaneId);
		}
	}
}
