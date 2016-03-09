using ColossalFramework;
using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.Data
{
    public static class TPPVehicleTypeConverter
    {
        public static ExtendedUnitType ConvertToUnitType(this TPPVehicleType vehicleType)
        {
            var result = ExtendedUnitType.None;

            foreach (var vehicleTypes in vehicleType.GetFlags())
            {
                switch (vehicleTypes)
                {
                    case TPPVehicleType.Unknown:
                        result |= ExtendedUnitType.Unknown;
                        break;
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
                    case TPPVehicleType.ServiceVehicles:
                        result |= ExtendedUnitType.ServiceVehicle;
                        break;
                    case TPPVehicleType.All:
                        result |= ExtendedUnitType.RoadVehicle;
                        break;
                    case TPPVehicleType.Tram:
                        result |= ExtendedUnitType.Tram;
                        break;
                    case TPPVehicleType.Metro:
                        result |= ExtendedUnitType.Metro;
                        break;
                    case TPPVehicleType.CargoTrain:
                        result |= ExtendedUnitType.CargoTrain;
                        break;
                    case TPPVehicleType.PassengerTrain:
                        result |= ExtendedUnitType.PassengerTrain;
                        break;
                    case TPPVehicleType.Train:
                        result |= ExtendedUnitType.Train;
                        break;
                    case TPPVehicleType.CargoShip:
                        result |= ExtendedUnitType.CargoShip;
                        break;
                    case TPPVehicleType.PassengerShip:
                        result |= ExtendedUnitType.PassengerShip;
                        break;
                    case TPPVehicleType.Ship:
                        result |= ExtendedUnitType.Ship;
                        break;
                    case TPPVehicleType.Plane:
                        result |= ExtendedUnitType.Plane;
                        break;
                    case TPPVehicleType.Bicycle:
                        result |= ExtendedUnitType.Bicycle;
                        break;
                    case TPPVehicleType.Citizen:
                        result |= ExtendedUnitType.Pedestrian;
                        break;
                    case TPPVehicleType.SnowTruck:
                        result |= ExtendedUnitType.SnowTruck;
                        break;
                    case TPPVehicleType.Taxi:
                        result |= ExtendedUnitType.Taxi;
                        break;
                }
            }

            return result;
        }
    }
}
