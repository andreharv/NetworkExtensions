using ColossalFramework;
using TrafficManager.Traffic;
using Transit.Framework.Network;

namespace Transit.Addon.TM.Data
{
    public static class ExtVehicleTypeExtensions
    {
        public static ExtendedUnitType ConvertToUnitType(this ExtVehicleType vehicleType)
        {
            var result = ExtendedUnitType.None;

            foreach (var vehicleTypes in vehicleType.GetFlags())
            {
                switch (vehicleTypes)
                {
                    case ExtVehicleType.Bicycle:
                        result |= ExtendedUnitType.Bicycle;
                        break;
                    case ExtVehicleType.Bus:
                        result |= ExtendedUnitType.Bus;
                        break;
                    case ExtVehicleType.CargoTrain:
                        result |= ExtendedUnitType.CargoTrain;
                        break;
                    case ExtVehicleType.CargoTruck:
                        result |= ExtendedUnitType.CargoTruck;
                        break;
                    case ExtVehicleType.Emergency:
                        result |= ExtendedUnitType.Emergency;
                        break;
                    case ExtVehicleType.PassengerCar:
                        result |= ExtendedUnitType.PassengerCar;
                        break;
                    case ExtVehicleType.PassengerTrain:
                        result |= ExtendedUnitType.PassengerTrain;
                        break;
                    case ExtVehicleType.Service:
                        result |= ExtendedUnitType.ServiceVehicle;
                        break;
                    case ExtVehicleType.Taxi:
                        result |= ExtendedUnitType.Taxi;
                        break;
                    case ExtVehicleType.Tram:
                        result |= ExtendedUnitType.Tram;
                        break;
                }
            }

            return result;
        }
    }
}