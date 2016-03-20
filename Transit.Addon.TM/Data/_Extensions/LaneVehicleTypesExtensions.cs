using ColossalFramework;
using TrafficManager;
using Transit.Addon.TM.Data.Legacy;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Addon.TM.Traffic;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.TM.Data
{
    public static class LaneVehicleTypesExtensions
    {
        public static TAMLaneRestriction ConvertToRestriction(this Configuration.LaneVehicleTypes laneVehicleTypes)
        {
            return new TAMLaneRestriction()
            {
                LaneId = laneVehicleTypes.laneId,
                UnitTypes = laneVehicleTypes.vehicleTypes.ConvertToUnitType()
            };
        }
    }
}