using System;

namespace Transit.Framework.Network
{
    [Flags]
    public enum ExtendedUnitType : long
    {
        None                    = 0,
        Unknown                 = 1 << 0,
        Pedestrian              = 1 << 1,
        Bicycle                 = 1 << 2,
        PassengerCar            = 1 << 3,
        CargoTruck              = 1 << 4,
        Ambulance               = 1 << 5,
        FireTruck               = 1 << 6,
        PoliceCar               = 1 << 7,
        Emergency               = 1 << 8,
        EmergencyVehicles       = Emergency | Ambulance | FireTruck | PoliceCar,
        GarbageTruck            = 1 << 9,
        Hearse                  = 1 << 10,
        SnowTruck               = 1 << 11,
        MaintenanceTruck        = 1 << 12,
        ServiceVehicles         = EmergencyVehicles | GarbageTruck | Hearse | SnowTruck | MaintenanceTruck,
        Bus                     = 1 << 13,
        Tram                    = 1 << 14,
        Taxi                    = 1 << 15,
        RoadPublicTransport     = Bus | Taxi,
        RoadVehicle             = PassengerCar | CargoTruck | EmergencyVehicles | ServiceVehicles | RoadPublicTransport,
        PassengerTrain          = 1 << 16,
        CargoTrain              = 1 << 17,
        Train                   = PassengerTrain | CargoTrain,
        PassengerShip           = 1 << 18,
        CargoShip               = 1 << 19,
        Ship                    = PassengerShip | CargoShip,
        PassengerPlane          = 1 << 20,
        Plane                   = PassengerPlane,
        Cargo                   = CargoTruck | CargoTrain | CargoShip,
        PublicTransport         = RoadPublicTransport | Tram | PassengerTrain,
        NonTransportRoadVehicle = RoadVehicle & ~PublicTransport
    }
}
