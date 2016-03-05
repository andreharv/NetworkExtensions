using ColossalFramework;
using System;
using System.Runtime.CompilerServices;
using ColossalFramework.Math;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.Network;
using UnityEngine;
using Transit.Framework.Redirection;

namespace CSL_Traffic
{
	public class CustomPassengerCarAI : CarAI
    {
        [RedirectFrom(typeof(PassengerCarAI))]
        public override void SimulationStep(ushort vehicleID, ref Vehicle data, Vector3 physicsLodRefPos)
        {
            if (((data.m_flags & Vehicle.Flags.Congestion) != Vehicle.Flags.None) &&
                ((Mod.Options & ModOptions.NoDespawn) != ModOptions.NoDespawn))
            {
                Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleID);
            }
            else
            {
                base.SimulationStep(vehicleID, ref data, physicsLodRefPos);
            }
        }
        
        [RedirectFrom(typeof(PassengerCarAI))]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData, Vector3 startPos, Vector3 endPos, bool startBothWays, bool endBothWays, bool undergroundTarget)
        {
            VehicleInfo info = this.m_info;
            ushort driverInstance = this.GetDriverInstance(vehicleID, ref vehicleData);
            if (driverInstance == 0)
            {
                return false;
            }
            CitizenManager instance = Singleton<CitizenManager>.instance;
            CitizenInfo info2 = instance.m_instances.m_buffer[(int)driverInstance].Info;
            NetInfo.LaneType laneTypes = NetInfo.LaneType.Vehicle | NetInfo.LaneType.Pedestrian;
            VehicleInfo.VehicleType vehicleType = this.m_info.m_vehicleType;
            bool allowUnderground = (vehicleData.m_flags & (Vehicle.Flags.Underground | Vehicle.Flags.Transition)) != Vehicle.Flags.None;
            PathUnit.Position startPosA;
            PathUnit.Position startPosB;
            float num;
            float num2;
            PathUnit.Position endPosA;
            if (PathManager.FindPathPosition(startPos, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, info.m_vehicleType, allowUnderground, false, 32f, out startPosA, out startPosB, out num, out num2) && info2.m_citizenAI.FindPathPosition(driverInstance, ref instance.m_instances.m_buffer[(int)driverInstance], endPos, laneTypes, vehicleType, undergroundTarget, out endPosA))
            {
                if (!startBothWays || num < 10f)
                {
                    startPosB = default(PathUnit.Position);
                }
                PathUnit.Position endPosB = default(PathUnit.Position);
                SimulationManager instance2 = Singleton<SimulationManager>.instance;
                uint path;
                if (Singleton<PathManager>.instance.CreatePath(ExtendedVehicleType.PassengerCar, out path, ref instance2.m_randomizer, instance2.m_currentBuildIndex, startPosA, startPosB, endPosA, endPosB, laneTypes, vehicleType, 20000f))
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
        [RedirectTo(typeof(PassengerCarAI))]
        private ushort GetDriverInstance(ushort vehicleID, ref Vehicle data)
        {
            throw new NotImplementedException("GetDriverInstance is target of redirection and is not implemented.");
        }
    }
}
