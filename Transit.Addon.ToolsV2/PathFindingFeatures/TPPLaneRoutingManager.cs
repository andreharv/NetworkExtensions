using System;
using Transit.Addon.ToolsV2.Data;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.PathFindingFeatures
{
    public class TPPLaneRoutingManager : ILaneRoutingManager
    {
        public bool CanLanesConnect(ushort nodeId, uint originLaneId, uint destinationLaneId, ExtendedUnitType vehicleType)
        {
            if ((vehicleType & TPPSupported.UNITS) == 0)
            {
				// unit type not supported
                return true;
            }


            var originLaneInfo = NetManager.instance.GetLaneInfo(originLaneId); // TODO query over segment id and lane index
            if (originLaneInfo == null)
            {
				// no lane info found
                return true;
            }

            if ((originLaneInfo.m_vehicleType & TPPSupported.VEHICLETYPES) == 0)
            {
				// vehicle type not supported
                return true;
            }


            var destinationLane = NetManager.instance.GetLaneInfo(destinationLaneId); // TODO query over segment id and lane index
			if (destinationLane == null)
            {
				// no lane info found
				return true;
            }

            if ((destinationLane.m_vehicleType & TPPSupported.VEHICLETYPES) == 0)
            {
				// vehicle type not supported
				return true;
            }

			TPPLaneDataV2 lane = TPPLaneDataManager.GetLane(originLaneId, false);
			if (lane == null)
				return true;

            return lane.ConnectsTo(destinationLaneId);
        }

		public bool CanLanesConnect(ushort nodeId, ushort segment1Id, byte lane1Index, uint lane1Id, ushort segment2Id, byte lane2Index, uint lane2Id, ExtendedUnitType unitType) {
			if ((unitType & TPPSupported.UNITS) == 0) {
				// unit type not supported
				return true;
			}


			var originLaneInfo = NetManager.instance.GetLaneInfo(segment1Id, lane1Index); // TODO query over segment id and lane index
			if (originLaneInfo == null) {
				// no lane info found
				return true;
			}

			if ((originLaneInfo.m_vehicleType & TPPSupported.VEHICLETYPES) == 0) {
				// vehicle type not supported
				return true;
			}

			var destinationLane = NetManager.instance.GetLaneInfo(segment2Id, lane2Index); // TODO query over segment id and lane index
			if (destinationLane == null) {
				// no lane info found
				return true;
			}

			if ((destinationLane.m_vehicleType & TPPSupported.VEHICLETYPES) == 0) {
				// vehicle type not supported
				return true;
			}

			TPPLaneDataV2 lane = TPPLaneDataManager.GetLane(lane1Id, false);
			if (lane == null)
				return true;

			return lane.ConnectsTo(lane2Id);
		}
	}
}
