using System;
using ColossalFramework;
using Transit.Framework.Network;

namespace TrafficManager.Traffic
{
    public static class ExtVehicleTypeConverter
    {
        public static ExtendedUnitType ConvertToUnitType(this ExtVehicleType vehicleType)
        {
            var result = ExtendedUnitType.None;

            foreach (var vehicleTypes in vehicleType.GetFlags())
            {
                switch (vehicleTypes)
                {
                    case ExtVehicleType.Emergency:
                        result |= ExtendedUnitType.Emergency;
                        break;
                    case ExtVehicleType.Service:
                        result |= ExtendedUnitType.ServiceVehicle;
                        break;
                    case ExtVehicleType.CargoTruck:
                        result |= ExtendedUnitType.CargoTruck;
                        break;
                    case ExtVehicleType.Taxi:
                        result |= ExtendedUnitType.Taxi;
                        break;
                    case ExtVehicleType.Bus:
                        result |= ExtendedUnitType.Bus;
                        break;
                    case ExtVehicleType.PassengerCar:
                        result |= ExtendedUnitType.PassengerCar;
                        break;

                    // TODO: Implement the rail restriction
                    //case ExtVehicleType.PassengerTrain:
                    //    result |= ExtendedUnitType.PassengerTrain;
                    //    break;
                    //case ExtVehicleType.CargoTrain:
                    //    result |= ExtendedUnitType.CargoTrain;
                    //    break;
                    //case ExtVehicleType.Tram:
                    //    result |= ExtendedUnitType.Tram;
                    //    break;
                    //case ExtVehicleType.Bicycle:
                    //    result |= ExtendedUnitType.Bicycle;
                    //    break;
                    //case ExtVehicleType.Pedestrian:
                    //    result |= ExtendedUnitType.Pedestrian;
                    //    break;
                    //case ExtVehicleType.PassengerShip:
                    //    result |= ExtendedUnitType.PassengerShip;
                    //    break;
                    //case ExtVehicleType.CargoShip:
                    //    result |= ExtendedUnitType.CargoShip;
                    //    break;
                    //case ExtVehicleType.PassengerPlane:
                    //    result |= ExtendedUnitType.PassengerPlane;
                    //    break;
                    //case ExtVehicleType.Ship:
                    //    result |= ExtendedUnitType.Ship;
                    //    break;
                    //case ExtVehicleType.CargoVehicle:
                    //    result |= ExtendedUnitType.Cargo;
                    //    break;
                    //case ExtVehicleType.PublicTransport:
                    //    result |= ExtendedUnitType.PublicTransport;
                    //    break;
                    //case ExtVehicleType.RoadPublicTransport:
                    //    result |= ExtendedUnitType.RoadPublicTransport;
                    //    break;
                    //case ExtVehicleType.RoadVehicle:
                    //    result |= ExtendedUnitType.RoadVehicle;
                    //    break;
                    //case ExtVehicleType.RailVehicle:
                    //    result |= ExtendedUnitType.Train;
                    //    break;
                    //case ExtVehicleType.NonTransportRoadVehicle:
                    //    result |= ExtendedUnitType.NonTransportRoadVehicle;
                    //    break;
                }
            }

            return result;
        }
    }
}
