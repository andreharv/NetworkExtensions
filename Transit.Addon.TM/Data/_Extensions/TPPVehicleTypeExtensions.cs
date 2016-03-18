using ColossalFramework;
using Transit.Addon.TM.Data.Legacy;
using Transit.Framework.Network;

namespace Transit.Addon.TM.Data
{
    public static class TPPVehicleTypeExtensions
    {
        public static ExtendedUnitType ConvertToUnitType(this TPPVehicleType vehicleType)
        {
            var result = ExtendedUnitType.None;

            foreach (var vehicleTypes in vehicleType.GetFlags())
            {
                switch (vehicleTypes)
                {
                    case TPPVehicleType.Ambulance:
                        result |= ExtendedUnitType.Ambulance;
                        break;
                    case TPPVehicleType.Bus:
                        result |= ExtendedUnitType.Bus;
                        break;
                    case TPPVehicleType.CargoTruck:
                        result |= ExtendedUnitType.CargoTruck;
                        break;
                    case TPPVehicleType.FireTruck:
                        result |= ExtendedUnitType.FireTruck;
                        break;
                    case TPPVehicleType.GarbageTruck:
                        result |= ExtendedUnitType.GarbageTruck;
                        break;
                    case TPPVehicleType.Hearse:
                        result |= ExtendedUnitType.Hearse;
                        break;
                    case TPPVehicleType.PassengerCar:
                        result |= ExtendedUnitType.PassengerCar;
                        break;
                    case TPPVehicleType.PoliceCar:
                        result |= ExtendedUnitType.PoliceCar;
                        break;
                    case TPPVehicleType.Emergency:
                        result |= ExtendedUnitType.Emergency;
                        break;
                    case TPPVehicleType.EmergencyVehicles:
                        result |= ExtendedUnitType.EmergencyVehicle;
                        break;
                    case TPPVehicleType.All:
                        result |= ExtendedUnitType.RoadVehicle;
                        break;
                }
            }

            return result;
        }
    }
}
