using System;
using ColossalFramework;
using Transit.Framework.Network;

namespace TrafficManager.Traffic
{
    public static class TPPVehicleTypeConverter
    {
        public static ExtendedUnitType ConvertToUnitType(this ExtVehicleType vehicleType)
        {
            var result = ExtendedUnitType.None;

            foreach (var vehicleTypes in vehicleType.GetFlags())
            {
                switch (vehicleTypes)
                {
                    case ExtVehicleType.None:
                        result |= ExtendedUnitType.None;
                        break;
                    case ExtVehicleType.PassengerCar:
                        result |= ExtendedUnitType.PassengerCar;
                        break;
                    case ExtVehicleType.Bus:
                        result |= ExtendedUnitType.Bus;
                        break;
                    case ExtVehicleType.Taxi:
                        result |= ExtendedUnitType.Taxi;
                        break;
                    case ExtVehicleType.CargoTruck:
                        result |= ExtendedUnitType.CargoTruck;
                        break;
                    case ExtVehicleType.Service:
                        result |= ExtendedUnitType.ServiceVehicle;
                        break;
                    case ExtVehicleType.Emergency:
                        result |= ExtendedUnitType.Emergency;
                        break;
                    case ExtVehicleType.PassengerTrain:
                        result |= ExtendedUnitType.PassengerTrain;
                        break;
                    case ExtVehicleType.CargoTrain:
                        result |= ExtendedUnitType.CargoTrain;
                        break;
                    case ExtVehicleType.Tram:
                        result |= ExtendedUnitType.Tram;
                        break;
                    case ExtVehicleType.Bicycle:
                        result |= ExtendedUnitType.Bicycle;
                        break;
                    case ExtVehicleType.Pedestrian:
                        result |= ExtendedUnitType.Pedestrian;
                        break;
                    case ExtVehicleType.PassengerShip:
                        result |= ExtendedUnitType.PassengerShip;
                        break;
                    case ExtVehicleType.CargoShip:
                        result |= ExtendedUnitType.CargoShip;
                        break;
                    case ExtVehicleType.PassengerPlane:
                        result |= ExtendedUnitType.PassengerPlane;
                        break;
                    case ExtVehicleType.Ship:
                        result |= ExtendedUnitType.Ship;
                        break;
                    case ExtVehicleType.CargoVehicle:
                        result |= ExtendedUnitType.Cargo;
                        break;
                    case ExtVehicleType.PublicTransport:
                        result |= ExtendedUnitType.PublicTransport;
                        break;
                    case ExtVehicleType.RoadPublicTransport:
                        result |= ExtendedUnitType.RoadPublicTransport;
                        break;
                    case ExtVehicleType.RoadVehicle:
                        result |= ExtendedUnitType.RoadVehicle;
                        break;
                    case ExtVehicleType.RailVehicle:
                        result |= ExtendedUnitType.Train;
                        break;
                    case ExtVehicleType.NonTransportRoadVehicle:
                        result |= ExtendedUnitType.NonTransportRoadVehicle;
                        break;
                }
            }

            return result;
        }

        public static ExtVehicleType ConvertToExtVehicleType(this ExtendedUnitType unitTypes)
        {
            var result = ExtVehicleType.None;

            foreach (var unitType in unitTypes.GetFlags())
            {
                switch (unitType)
                {
                    case ExtendedUnitType.None:
                        result |= ExtVehicleType.None;
                        break;
                    case ExtendedUnitType.Unknown:
                        result |= ExtVehicleType.Unknown;
                        break;
                    case ExtendedUnitType.Pedestrian:
                        result |= ExtVehicleType.Pedestrian;
                        break;
                    case ExtendedUnitType.Bicycle:
                        result |= ExtVehicleType.Bicycle;
                        break;
                    case ExtendedUnitType.PassengerCar:
                        result |= ExtVehicleType.PassengerCar;
                        break;
                    case ExtendedUnitType.CargoTruck:
                        result |= ExtVehicleType.CargoTruck;
                        break;
                    case ExtendedUnitType.Ambulance:
                        result |= ExtVehicleType.Service;
                        break;
                    case ExtendedUnitType.FireTruck:
                        result |= ExtVehicleType.Service;
                        break;
                    case ExtendedUnitType.PoliceCar:
                        result |= ExtVehicleType.Service;
                        break;
                    case ExtendedUnitType.Emergency:
                        result |= ExtVehicleType.Emergency;
                        break;
                    case ExtendedUnitType.EmergencyVehicle:
                        result |= ExtVehicleType.Service;
                        break;
                    case ExtendedUnitType.GarbageTruck:
                        result |= ExtVehicleType.Service;
                        break;
                    case ExtendedUnitType.Hearse:
                        result |= ExtVehicleType.Service;
                        break;
                    case ExtendedUnitType.SnowTruck:
                        result |= ExtVehicleType.Service;
                        break;
                    case ExtendedUnitType.MaintenanceTruck:
                        result |= ExtVehicleType.Service;
                        break;
                    case ExtendedUnitType.UnknownService:
                        result |= ExtVehicleType.Service;
                        break;
                    case ExtendedUnitType.ServiceVehicle:
                        result |= ExtVehicleType.Service;
                        break;
                    case ExtendedUnitType.Bus:
                        result |= ExtVehicleType.Bus;
                        break;
                    case ExtendedUnitType.Tram:
                        result |= ExtVehicleType.Tram;
                        break;
                    case ExtendedUnitType.Metro:
                        result |= ExtVehicleType.Unknown;
                        break;
                    case ExtendedUnitType.Taxi:
                        result |= ExtVehicleType.Taxi;
                        break;
                    case ExtendedUnitType.RoadPublicTransport:
                        result |= ExtVehicleType.RoadPublicTransport;
                        break;
                    case ExtendedUnitType.RoadVehicle:
                        result |= ExtVehicleType.RoadVehicle;
                        break;
                    case ExtendedUnitType.PassengerTrain:
                        result |= ExtVehicleType.PassengerTrain;
                        break;
                    case ExtendedUnitType.CargoTrain:
                        result |= ExtVehicleType.CargoTrain;
                        break;
                    case ExtendedUnitType.Train:
                        result |= ExtVehicleType.RailVehicle;
                        break;
                    case ExtendedUnitType.PassengerShip:
                        result |= ExtVehicleType.PassengerShip;
                        break;
                    case ExtendedUnitType.CargoShip:
                        result |= ExtVehicleType.CargoShip;
                        break;
                    case ExtendedUnitType.Ship:
                        result |= ExtVehicleType.Ship;
                        break;
                    case ExtendedUnitType.PassengerPlane:
                        result |= ExtVehicleType.PassengerPlane;
                        break;
                    case ExtendedUnitType.Cargo:
                        result |= ExtVehicleType.CargoVehicle;
                        break;
                    case ExtendedUnitType.PublicTransport:
                        result |= ExtVehicleType.PublicTransport;
                        break;
                    case ExtendedUnitType.NonTransportRoadVehicle:
                        result |= ExtVehicleType.NonTransportRoadVehicle;
                        break;
                }
            }

            return result;
        }
    }
}
