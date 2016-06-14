using System;
using System.Runtime.CompilerServices;
using ColossalFramework;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.Network;
using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;
using UnityEngine;

namespace Transit.Framework.Hooks.AI.Units
{
	public class PassengerCarAIHook : CarAI
    {
        [RedirectFrom(typeof(PassengerCarAI), (ulong)PrerequisiteType.PathFinding)]
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
            bool allowUnderground = (vehicleData.m_flags & (Vehicle.Flags.Underground | Vehicle.Flags.Transition)) != 0;
            bool randomParking = false;
            ushort targetBuilding = instance.m_instances.m_buffer[(int)driverInstance].m_targetBuilding;
            if (targetBuilding != 0 && Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)targetBuilding].Info.m_class.m_service > ItemClass.Service.Office)
            {
                randomParking = true;
            }
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
                if (Singleton<PathManager>.instance.CreatePath(ExtendedUnitType.PassengerCar, out path, ref instance2.m_randomizer, instance2.m_currentBuildIndex, startPosA, startPosB, endPosA, endPosB, default(PathUnit.Position), laneTypes, vehicleType, 20000f, false, false, false, false, randomParking))
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
        [RedirectTo(typeof(PassengerCarAI), (ulong)PrerequisiteType.PathFinding)]
        private ushort GetDriverInstance(ushort vehicleID, ref Vehicle data)
        {
            throw new NotImplementedException("GetDriverInstance is target of redirection and is not implemented.");
        }
    }
}
