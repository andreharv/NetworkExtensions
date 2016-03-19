using System;
using Transit.Addon.TM.Data;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public class TPPLaneRoutingManager : ILaneRoutingManager
    {
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

			TPPLaneDataV2 lane = TPPDataManager.instance.GetLane(originLaneId);
			if (lane == null)
				return true;

			return lane.ConnectsTo(destinationLaneId);
		}
	}
}
