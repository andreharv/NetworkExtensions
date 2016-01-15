using Transit.Framework.Unsafe;
using UnityEngine;
using VehicleType = VehicleInfo.VehicleType;

namespace Transit.Addon.TrafficAI
{
    public class CustomCargoTruckAI : CarAI
    {
        [RedirectFrom(typeof(CargoTruckAI))]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData, Vector3 startPos, Vector3 endPos, bool startBothWays, bool endBothWays)
        {
            if ((vehicleData.m_flags & (Vehicle.Flags.TransferToSource | Vehicle.Flags.GoingBack)) != Vehicle.Flags.None)
            {
                return base.StartPathFind(vehicleID, ref vehicleData, startPos, endPos, startBothWays, endBothWays);
            }

            bool allowUnderground = (vehicleData.m_flags & (Vehicle.Flags.Underground | Vehicle.Flags.Transition)) != Vehicle.Flags.None;
            PathUnit.Position startPosA;
            PathUnit.Position startPosB;
            float num;
            float num2;
            bool flag = PathManager.FindPathPosition(startPos, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, VehicleInfo.VehicleType.Car, allowUnderground, false, 32f, out startPosA, out startPosB, out num, out num2);
            PathUnit.Position position;
            PathUnit.Position position2;
            float num3;
            float num4;
            if (PathManager.FindPathPosition(startPos, ItemClass.Service.PublicTransport, NetInfo.LaneType.Vehicle, VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Ship, allowUnderground, false, 32f, out position, out position2, out num3, out num4))
            {
                if (!flag || num3 < num)
                {
                    startPosA = position;
                    startPosB = position2;
                    num = num3;
                    num2 = num4;
                }
                flag = true;
            }
            PathUnit.Position endPosA;
            PathUnit.Position endPosB;
            float num5;
            float num6;
            bool flag2 = PathManager.FindPathPosition(endPos, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, VehicleInfo.VehicleType.Car, false, false, 32f, out endPosA, out endPosB, out num5, out num6);
            PathUnit.Position position3;
            PathUnit.Position position4;
            float num7;
            float num8;
            if (PathManager.FindPathPosition(endPos, ItemClass.Service.PublicTransport, NetInfo.LaneType.Vehicle, VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Ship, false, false, 32f, out position3, out position4, out num7, out num8))
            {
                if (!flag2 || num7 < num5)
                {
                    endPosA = position3;
                    endPosB = position4;
                    num5 = num7;
                    num6 = num8;
                }
                flag2 = true;
            }
            if (flag && flag2)
            {
                PathManager instance = PathManager.instance;
                if (!startBothWays || num < 10f)
                {
                    startPosB = default(PathUnit.Position);
                }
                if (!endBothWays || num5 < 10f)
                {
                    endPosB = default(PathUnit.Position);
                }
                NetInfo.LaneType laneTypes = NetInfo.LaneType.Vehicle | NetInfo.LaneType.CargoVehicle;
                VehicleType vehicleTypes = VehicleType.Car | VehicleType.Train | VehicleType.Ship | (VehicleType)ExtendedVehicleType.CargoTruck;
                uint path;
                if (instance.CreatePath(out path, ref SimulationManager.instance.m_randomizer, SimulationManager.instance.m_currentBuildIndex, startPosA, startPosB, endPosA, endPosB, laneTypes, vehicleTypes, 20000f, this.IsHeavyVehicle(), this.IgnoreBlocked(vehicleID, ref vehicleData), false, false))
                {
                    if (vehicleData.m_path != 0u)
                    {
                        instance.ReleasePath(vehicleData.m_path);
                    }
                    vehicleData.m_path = path;
                    vehicleData.m_flags |= Vehicle.Flags.WaitingPath;
                    return true;
                }
            }
            return false;
        }
    }
}
