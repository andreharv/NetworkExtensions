using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.PathFinding.ExtendedFeatures
{
    public static class TPPSupported
    {
        public const ExtendedUnitType UNITS =
            ExtendedUnitType.ServiceVehicle |
            ExtendedUnitType.PassengerCar |
            ExtendedUnitType.CargoTruck |
            ExtendedUnitType.Bus |
            ExtendedUnitType.Taxi;

        public const VehicleInfo.VehicleType VEHICLETYPES =
            VehicleInfo.VehicleType.Car |
            VehicleInfo.VehicleType.Train;
    }
}
