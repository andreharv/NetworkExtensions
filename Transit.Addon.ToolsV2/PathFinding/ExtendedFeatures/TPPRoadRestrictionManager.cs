using CSL_Traffic;
using Transit.Addon.ToolsV2.Data;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.PathFinding.ExtendedFeatures
{
    public class TPPRoadRestrictionManager : IRoadRestrictionManager
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

        public bool CanUseLane(uint laneId, ExtendedVehicleType vehicleType)
        {
            if ((vehicleType & ROUTED_UNITS) == 0)
            {
                return true;
            }

            var laneInfo = NetManager.instance.GetLaneInfo(laneId);

            if (laneInfo == null)
            {
                return true;
            }

            if ((laneInfo.m_vehicleType & ROUTED_VEHICLETYPES) == 0)
            {
                return true;
            }

            // T++ 
            return (TPPLaneDataManager.GetLane(laneId).m_vehicleTypes & vehicleType) != ExtendedVehicleType.None;

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
