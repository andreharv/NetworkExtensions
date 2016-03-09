using System;
using ColossalFramework;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.Network;
using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;
using UnityEngine;

namespace Transit.Framework.Hooks.AI.Units
{
    public class CitizenAIHook : CitizenAI
    {
        [RedirectFrom(typeof(CitizenAI), (ulong)PrerequisiteType.PathFinding)]
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
                PathManager.FindPathPosition(position, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, VehicleInfo.VehicleType.Car, false, false, 32f, out vehiclePosition);
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
                if (Singleton<PathManager>.instance.CreatePath(ExtendedUnitType.PassengerCar, out path, ref Singleton<SimulationManager>.instance.m_randomizer, Singleton<SimulationManager>.instance.m_currentBuildIndex, startPosA, position2, endPosA, position2, vehiclePosition, laneType, vehicleType, 20000f, false, false, false, false))
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
    }
}
