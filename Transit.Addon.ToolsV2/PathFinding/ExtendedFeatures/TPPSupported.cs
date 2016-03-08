using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.PathFinding.ExtendedFeatures
{
    public static class TPPSupported
    {
        public const ExtendedVehicleType UNITS =
            ExtendedVehicleType.ServiceVehicles |
            ExtendedVehicleType.PassengerCar |
            ExtendedVehicleType.CargoTruck |
            ExtendedVehicleType.Bus |
            ExtendedVehicleType.Taxi;

        public const VehicleInfo.VehicleType VEHICLETYPES =
            VehicleInfo.VehicleType.Car |
            VehicleInfo.VehicleType.Train;
    }
}
