using System;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.Math;
using Transit.Framework.Network;
using Transit.Framework.Redirection;
using UnityEngine;

namespace CSL_Traffic
{
    public class CustomCitizenAI : CitizenAI
    {
        [RedirectFrom(typeof(CitizenAI))]
        protected new bool StartPathFind(ushort instanceID, ref CitizenInstance citizenData, Vector3 startPos, Vector3 endPos, VehicleInfo vehicleInfo)
        {
            NetInfo.LaneType laneType = NetInfo.LaneType.Pedestrian;
            VehicleInfo.VehicleType vehicleType = VehicleInfo.VehicleType.None;
            if (vehicleInfo != null)
            {
                if (vehicleInfo.m_class.m_subService == ItemClass.SubService.PublicTransportTaxi)
                {
                    if ((citizenData.m_flags & CitizenInstance.Flags.CannotUseTaxi) == CitizenInstance.Flags.None && Singleton<DistrictManager>.instance.m_districts.m_buffer[0].m_productionData.m_finalTaxiCapacity != 0u)
                    {
                        SimulationManager instance = Singleton<SimulationManager>.instance;
                        if (instance.m_isNightTime || instance.m_randomizer.Int32(2u) == 0)
                        {
                            laneType |= (NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle);
                            vehicleType |= vehicleInfo.m_vehicleType;
                        }
                    }
                }
                else
                {
                    laneType |= NetInfo.LaneType.Vehicle;
                    vehicleType |= vehicleInfo.m_vehicleType;
                }
            }
            PathUnit.Position vehiclePosition = default(PathUnit.Position);
            ushort parkedVehicle = Singleton<CitizenManager>.instance.m_citizens.m_buffer[(int)((UIntPtr)citizenData.m_citizen)].m_parkedVehicle;
            if (parkedVehicle != 0)
            {
                Vector3 position = Singleton<VehicleManager>.instance.m_parkedVehicles.m_buffer[(int)parkedVehicle].m_position;
                ExtendedPathManager.FindPathPosition(ExtendedVehicleType.PassengerCar, position, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, VehicleInfo.VehicleType.Car, false, false, 32f, out vehiclePosition);
            }
            bool allowUnderground = (citizenData.m_flags & (CitizenInstance.Flags.Underground | CitizenInstance.Flags.Transition)) != CitizenInstance.Flags.None;
            PathUnit.Position startPosA;
            PathUnit.Position endPosA;
            if (this.FindPathPosition(instanceID, ref citizenData, startPos, laneType, vehicleType, allowUnderground, out startPosA) && this.FindPathPosition(instanceID, ref citizenData, endPos, laneType, vehicleType, false, out endPosA))
            {
                if ((citizenData.m_flags & CitizenInstance.Flags.CannotUseTransport) == CitizenInstance.Flags.None)
                {
                    laneType |= NetInfo.LaneType.PublicTransport;
                }
                PathUnit.Position position2 = default(PathUnit.Position);
                uint path;
                if (Singleton<PathManager>.instance.CreatePath(ExtendedVehicleType.PassengerCar, out path, ref Singleton<SimulationManager>.instance.m_randomizer, Singleton<SimulationManager>.instance.m_currentBuildIndex, startPosA, position2, endPosA, position2, vehiclePosition, laneType, vehicleType, 20000f, false, false, false, false))
                {
                    if (citizenData.m_path != 0u)
                    {
                        Singleton<PathManager>.instance.ReleasePath(citizenData.m_path);
                    }
                    citizenData.m_path = path;
                    citizenData.m_flags |= CitizenInstance.Flags.WaitingPath;
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectFrom(typeof(CitizenAI))]
        public override bool FindPathPosition(ushort instanceID, ref CitizenInstance citizenData, Vector3 pos, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, bool allowUnderground, out PathUnit.Position position)
        {
            position = default(PathUnit.Position);
            float num = 1E+10f;
            PathUnit.Position position2;
            PathUnit.Position position3;
            float num2;
            float num3;
            if (ExtendedPathManager.FindPathPosition(ExtendedVehicleType.PassengerCar, pos, ItemClass.Service.Road, laneTypes, vehicleTypes, allowUnderground, false, 32f, out position2, out position3, out num2, out num3) && num2 < num)
            {
                num = num2;
                position = position2;
            }
            PathUnit.Position position4;
            PathUnit.Position position5;
            float num4;
            float num5;
            if (ExtendedPathManager.FindPathPosition(ExtendedVehicleType.PassengerCar, pos, ItemClass.Service.Beautification, laneTypes, vehicleTypes, allowUnderground, false, 32f, out position4, out position5, out num4, out num5) && num4 < num)
            {
                num = num4;
                position = position4;
            }
            PathUnit.Position position6;
            PathUnit.Position position7;
            float num6;
            float num7;
            if ((citizenData.m_flags & CitizenInstance.Flags.CannotUseTransport) == CitizenInstance.Flags.None && ExtendedPathManager.FindPathPosition(ExtendedVehicleType.PassengerCar, pos, ItemClass.Service.PublicTransport, laneTypes, vehicleTypes, allowUnderground, false, 32f, out position6, out position7, out num6, out num7) && num6 < num)
            {
                position = position6;
            }
            return position.m_segment != 0;
        }
    }
}
