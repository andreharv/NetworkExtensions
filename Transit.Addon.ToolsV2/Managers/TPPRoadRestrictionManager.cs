using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using Transit.Framework.Network;

namespace CSL_Traffic
{
    public class TPPRoadRestrictionManager : Singleton<TPPRoadRestrictionManager>
    {
        private const ExtendedVehicleType ROUTED_UNITS =
            ExtendedVehicleType.ServiceVehicles |
            ExtendedVehicleType.PassengerCar |
            ExtendedVehicleType.CargoTruck |
            ExtendedVehicleType.Bus |
            ExtendedVehicleType.Taxi;

        private const VehicleInfo.VehicleType ROUTED_VEHICLETYPES =
            VehicleInfo.VehicleType.Car |
            VehicleInfo.VehicleType.Train;

        public bool CanUseLane(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedVehicleType vehicleType)
        {
            if ((vehicleType & ROUTED_UNITS) == 0)
            {
                return true;
            }

            if (laneInfo == null)
            {
                laneInfo = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].Info.m_lanes[laneIndex];
            }

            if ((laneInfo.m_vehicleType & ROUTED_VEHICLETYPES) == 0)
            {
                return true;
            }

            // T++ 
            return (LaneManager.GetLane(laneId).m_vehicleTypes & vehicleType) != ExtendedVehicleType.None;

            // TM
//            ExtVehicleType allowedTypes = VehicleRestrictionsManager.GetAllowedVehicleTypes(segmentId, laneIndex, laneId, laneInfo);
//#if DEBUGPF
//			if (debug) {
//				Log._Debug($"CanUseLane: segmentId={segmentId} laneIndex={laneIndex} laneId={laneId}, _extVehicleType={_extVehicleType} _vehicleTypes={_vehicleTypes} _laneTypes={_laneTypes} _transportVehicle={_transportVehicle} _isHeavyVehicle={_isHeavyVehicle} allowedTypes={allowedTypes} res={((allowedTypes & _extVehicleType) != ExtVehicleType.None)}");
//            }
//#endif

//            return ((allowedTypes & _extVehicleType) != ExtVehicleType.None);
        }
    }
}
