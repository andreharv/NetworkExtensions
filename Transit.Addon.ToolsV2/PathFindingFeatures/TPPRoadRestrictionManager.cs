using Transit.Addon.ToolsV2.Data;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.PathFindingFeatures
{
    public class TPPRoadRestrictionManager : IRoadRestrictionManager
    {
        public bool CanUseLane(uint laneId, ExtendedUnitType unitType)
        {
            if ((unitType & TPPSupported.UNITS) == 0)
            {
                return true;
            }

            var laneInfo = NetManager.instance.GetLaneInfo(laneId);

            if (laneInfo == null)
            {
                return true;
            }

            if ((laneInfo.m_vehicleType & TPPSupported.VEHICLETYPES) == 0)
            {
                return true;
            }

            // T++ 
            return (TPPLaneDataManager.GetLane(laneId).m_unitTypes & unitType) != ExtendedUnitType.None;

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
