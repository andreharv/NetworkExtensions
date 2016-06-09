using System;
using System.Runtime.CompilerServices;
using ColossalFramework;
using Transit.Framework.Network;
using Transit.Framework.Redirection;
using UnityEngine;

namespace CSL_Traffic.AI.Redirections
{
    public class CustomTaxiAI : CarAI
    {
        [RedirectFrom(typeof(TaxiAI))]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData, Vector3 startPos, Vector3 endPos, bool startBothWays, bool endBothWays, bool undergroundTarget)
        {
            CitizenManager instance = Singleton<CitizenManager>.instance;
            ushort passengerInstance = this.GetPassengerInstance(vehicleID, ref vehicleData);
            if (passengerInstance == 0 || (instance.m_instances.m_buffer[(int)passengerInstance].m_flags & CitizenInstance.Flags.Character) != CitizenInstance.Flags.None)
            {
                return base.StartPathFind(vehicleID, ref vehicleData, startPos, endPos, startBothWays, endBothWays, undergroundTarget);
            }
            VehicleInfo info = this.m_info;
            CitizenInfo info2 = instance.m_instances.m_buffer[(int)passengerInstance].Info;
            NetInfo.LaneType laneType = NetInfo.LaneType.Vehicle | NetInfo.LaneType.Pedestrian | NetInfo.LaneType.TransportVehicle;
            VehicleInfo.VehicleType vehicleType = this.m_info.m_vehicleType;
            bool allowUnderground = (vehicleData.m_flags & (Vehicle.Flags.Underground | Vehicle.Flags.Transition)) != 0;
            PathUnit.Position startPosA;
            PathUnit.Position startPosB;
            float num;
            float num2;
            PathUnit.Position endPosA;
            if (PathManager.FindPathPosition(startPos, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, info.m_vehicleType, allowUnderground, false, 32f, out startPosA, out startPosB, out num, out num2) && info2.m_citizenAI.FindPathPosition(passengerInstance, ref instance.m_instances.m_buffer[(int)passengerInstance], endPos, laneType, vehicleType, undergroundTarget, out endPosA))
            {
                if ((instance.m_instances.m_buffer[(int)passengerInstance].m_flags & CitizenInstance.Flags.CannotUseTransport) == CitizenInstance.Flags.None)
                {
                    laneType |= NetInfo.LaneType.PublicTransport;
                }
                if (!startBothWays || num < 10f)
                {
                    startPosB = default(PathUnit.Position);
                }
                PathUnit.Position endPosB = default(PathUnit.Position);
                SimulationManager instance2 = Singleton<SimulationManager>.instance;
                uint path;
                if (Singleton<PathManager>.instance.CreatePath(ExtendedVehicleType.Taxi, out path, ref instance2.m_randomizer, instance2.m_currentBuildIndex, startPosA, startPosB, endPosA, endPosB, laneType, vehicleType, 20000f))
                {
                    if (vehicleData.m_path != 0u)
                    {
                        Singleton<PathManager>.instance.ReleasePath(vehicleData.m_path);
                    }
                    vehicleData.m_path = path;
                    vehicleData.m_flags |= Vehicle.Flags.WaitingPath;
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(TaxiAI))]
	    private ushort GetPassengerInstance(ushort vehicleID, ref Vehicle data)
        {
            throw new NotImplementedException("GetPassengerInstance is target of redirection and is not implemented.");
        }
    }
}
