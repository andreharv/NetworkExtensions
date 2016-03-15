using System;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Addon.TM.Traffic;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace TrafficManager.Custom.PathFindingFeatures {
	public class TMRoadRestrictionManager : IRoadRestrictionManager {

		// Warning: Avoid using this method due to performance considerations.
		public bool CanUseLane(uint laneId, ExtendedUnitType unitType) {
			if ((unitType & TMSupported.UNITS) == 0) {
				return true;
			}

			var laneInfo = NetManager.instance.GetLaneInfo(laneId);

			if (laneInfo == null) {
				return true;
			}

			if ((laneInfo.m_vehicleType & TMSupported.VEHICLETYPES) == 0) {
				return true;
			}

			ushort? segmentId = NetManager.instance.GetLaneNetSegmentId(laneId);
			if (segmentId == null) {
				throw new Exception("TM: Segment not found for LaneID " + laneId);
			}

			byte? laneIndex = NetManager.instance.GetLaneIndex(laneId);
			if (laneIndex == null) {
				throw new Exception("TM: LaneIndex not found for LaneID " + laneId);
			}

			TMVehicleType allowedVehicleTypes = VehicleRestrictionsManager.GetAllowedVehicleTypes(segmentId.Value, laneIndex.Value, laneInfo);
			ExtendedUnitType allowedUnitTypes = allowedVehicleTypes.ConvertToUnitType(); // TODO remove type ExtVehicleType

#if DEBUGPF
            	if (debug) {
            		Log._Debug($"CanUseLane: segmentId={segmentId} laneIndex={laneIndex} laneId={laneId}, unitType={unitType} _vehicleTypes={_vehicleTypes} _laneTypes={_laneTypes} _transportVehicle={_transportVehicle} _isHeavyVehicle={_isHeavyVehicle} allowedTypes={allowedTypes} res={((allowedTypes & unitType) != ExtVehicleType.None)}");
                }
#endif

			return ((allowedUnitTypes & unitType) != 0);
		}

		public bool CanUseLane(ushort segmentId, byte laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedUnitType unitType) {
			if ((unitType & TMSupported.UNITS) == 0) {
				// unsupported type. allow.
				return true;
			}

			TMVehicleType allowedVehicleTypes = VehicleRestrictionsManager.GetAllowedVehicleTypes(segmentId, laneIndex, laneInfo);
			ExtendedUnitType allowedUnitTypes = allowedVehicleTypes.ConvertToUnitType(); // TODO remove type ExtVehicleType

			return ((allowedUnitTypes & unitType) != 0);
		}
	}
}
