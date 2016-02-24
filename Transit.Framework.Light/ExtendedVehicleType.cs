using System;

// Legacy from T++
namespace Transit.Framework.Light
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
        CargoShip      = 1 << 20,
        PassengerShip  = 1 << 21,
        Plane          = 1 << 22,
        Bicycle        = 1 << 23,
    }

    //public static class VehicleTypeExtensions
    //{
    //    public static ExtendedVehicleType ToExtendedVehicleType(this VehicleInfo.VehicleType vehicleType, NetInfo.LaneType laneType, bool isHeavyVehicle)
    //    {
    //        if (vehicleType.HasFlag(VehicleInfo.VehicleType.Car))
    //        {
    //            if (laneType.HasFlag(NetInfo.LaneType.CargoVehicle) || isHeavyVehicle)
    //            {
    //                return ExtendedVehicleType.CargoTruck;
    //            }
    //            if (laneType.HasFlag(NetInfo.LaneType.PublicTransport))
    //            {
    //                return ExtendedVehicleType.Bus;
    //            }
    //            if (laneType.HasFlag(NetInfo.LaneType.TransportVehicle))
    //            {
    //                return ExtendedVehicleType.EmergencyVehicles;
    //            }
    //            return ExtendedVehicleType.PassengerCar;
    //        }

    //        if (vehicleType.HasFlag(VehicleInfo.VehicleType.Metro))
    //        {
    //            return ExtendedVehicleType.Metro;
    //        }

    //        if (vehicleType.HasFlag(VehicleInfo.VehicleType.Train))
    //        {
    //            return ExtendedVehicleType.PassengerTrain | ExtendedVehicleType.CargoTrain;
    //        }

    //        if (vehicleType.HasFlag(VehicleInfo.VehicleType.Ship))
    //        {
    //            return ExtendedVehicleType.PassengerShip | ExtendedVehicleType.CargoShip;
    //        }

    //        if (vehicleType.HasFlag(VehicleInfo.VehicleType.Plane))
    //        {
    //            return ExtendedVehicleType.Plane;
    //        }

    //        if (vehicleType.HasFlag(VehicleInfo.VehicleType.Bicycle))
    //        {
    //            return ExtendedVehicleType.Bicycle;
    //        }

    //        if (vehicleType.HasFlag(VehicleInfo.VehicleType.Tram))
    //        {
    //            return ExtendedVehicleType.Tram;
    //        }

    //        return ExtendedVehicleType.None;
    //    }
    //}
}
