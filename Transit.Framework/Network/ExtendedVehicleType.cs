using System;

namespace Transit.Framework.Network
{
    [Flags]
    public enum ExtendedVehicleType
    {
        None = 0,

        Ambulance    = 1 << 0,
        Bus          = 1 << 1,
        CargoTruck   = 1 << 2,
        FireTruck    = 1 << 3,
        GarbageTruck = 1 << 4,
        Hearse       = 1 << 5,
        PassengerCar = 1 << 6,
        PoliceCar    = 1 << 7,

        Emergency    = 1 << 15,
        EmergencyVehicles = Emergency | Ambulance | FireTruck | PoliceCar,
        ServiceVehicles = EmergencyVehicles | GarbageTruck | Hearse,

        All = ServiceVehicles | PassengerCar | CargoTruck | Bus,

        Tram           = 1 << 16,
        Metro          = 1 << 17,
        CargoTrain     = 1 << 18,
        PassengerTrain = 1 << 19,
        Train          = CargoTrain | PassengerTrain,
        CargoShip      = 1 << 20,
        PassengerShip  = 1 << 21,
        Ship           = CargoShip | PassengerShip,
        Plane          = 1 << 22,
        Bicycle        = 1 << 23,
        Citizen        = 1 << 24,
        SnowTruck      = 1 << 25,
        Taxi           = 1 << 26,

        Unknown        = 1 << 31,
    }
}
