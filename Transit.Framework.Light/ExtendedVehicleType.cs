using System;

// Legacy from T++
namespace Transit.Framework.Light
{
    [Flags]
    public enum ExtendedVehicleType
    {
        None = 0,

        Ambulance = 1,
        Bus = 2,
        CargoTruck = 4,
        FireTruck = 8,
        GarbageTruck = 16,
        Hearse = 32,
        PassengerCar = 64,
        PoliceCar = 128,

        Emergency = 32768,
        EmergencyVehicles = Emergency | Ambulance | FireTruck | PoliceCar,
        ServiceVehicles = EmergencyVehicles | GarbageTruck | Hearse,

        All = ServiceVehicles | PassengerCar | CargoTruck | Bus
    }
}
