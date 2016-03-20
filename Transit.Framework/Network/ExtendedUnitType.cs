using System;

namespace Transit.Framework.Network
{
    [Flags]
    public enum ExtendedUnitType : ulong
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
        EmergencyVehicle        = Ambulance | FireTruck | PoliceCar,
        GarbageTruck            = 1 << 9,
        Hearse                  = 1 << 10,
        SnowTruck               = 1 << 11,
        MaintenanceTruck        = 1 << 12,
        ServiceVehicle          = EmergencyVehicle | GarbageTruck | Hearse | SnowTruck | MaintenanceTruck,
        Bus                     = 1 << 14,
        Tram                    = 1 << 15,
        Metro                   = 1 << 16,
        Taxi                    = 1 << 17,
        RoadPublicTransport     = Bus | Taxi,
        RoadVehicle             = PassengerCar | CargoTruck | EmergencyVehicle | ServiceVehicle | RoadPublicTransport,
        PassengerTrain          = 1 << 18,
        CargoTrain              = 1 << 19,
        Train                   = PassengerTrain | CargoTrain,
        PassengerShip           = 1 << 20,
        CargoShip               = 1 << 21,
        Ship                    = PassengerShip | CargoShip,
        PassengerPlane          = 1 << 22,
        Plane                   = PassengerPlane,
        Cargo                   = CargoTruck | CargoTrain | CargoShip,
        PublicTransport         = RoadPublicTransport | Tram | Metro | PassengerTrain,
        NonTransportRoadVehicle = RoadVehicle & ~RoadPublicTransport
    }
}
