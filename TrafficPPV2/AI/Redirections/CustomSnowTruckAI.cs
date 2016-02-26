using System;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.Math;
using Transit.Framework.Network;
using Transit.Framework.Redirection;
using UnityEngine;

namespace CSL_Traffic
{
    public class CustomSnowTruckAI : CarAI
    {
        [RedirectFrom(typeof(SnowTruckAI))]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData)
        {
            if ((vehicleData.m_flags & Vehicle.Flags.WaitingTarget) != Vehicle.Flags.None)
            {
                return true;
            }
            if ((vehicleData.m_flags & Vehicle.Flags.GoingBack) != Vehicle.Flags.None)
            {
                if (vehicleData.m_sourceBuilding != 0)
                {
                    BuildingManager instance = Singleton<BuildingManager>.instance;
                    BuildingInfo info = instance.m_buildings.m_buffer[(int)vehicleData.m_sourceBuilding].Info;
                    Randomizer randomizer = new Randomizer((int)vehicleID);
                    Vector3 vector;
                    Vector3 endPos;
                    info.m_buildingAI.CalculateUnspawnPosition(vehicleData.m_sourceBuilding, ref instance.m_buildings.m_buffer[(int)vehicleData.m_sourceBuilding], ref randomizer, this.m_info, out vector, out endPos);
                    return this.StartPathFind(ExtendedVehicleType.SnowTruck, vehicleID, ref vehicleData, vehicleData.m_targetPos3, endPos, IsHeavyVehicle(), IgnoreBlocked(vehicleID, ref vehicleData));
                }
            }
            else if ((vehicleData.m_flags & Vehicle.Flags.TransferToTarget) != Vehicle.Flags.None)
            {
                if (vehicleData.m_targetBuilding != 0)
                {
                    BuildingManager instance2 = Singleton<BuildingManager>.instance;
                    BuildingInfo info2 = instance2.m_buildings.m_buffer[(int)vehicleData.m_targetBuilding].Info;
                    Randomizer randomizer2 = new Randomizer((int)vehicleID);
                    Vector3 vector2;
                    Vector3 endPos2;
                    info2.m_buildingAI.CalculateUnspawnPosition(vehicleData.m_targetBuilding, ref instance2.m_buildings.m_buffer[(int)vehicleData.m_targetBuilding], ref randomizer2, this.m_info, out vector2, out endPos2);
                    return this.StartPathFind(ExtendedVehicleType.SnowTruck, vehicleID, ref vehicleData, vehicleData.m_targetPos3, endPos2, IsHeavyVehicle(), IgnoreBlocked(vehicleID, ref vehicleData));
                }
            }
            else if (vehicleData.m_targetBuilding != 0 && this.CheckTargetSegment(vehicleID, ref vehicleData))
            {
                NetManager instance3 = Singleton<NetManager>.instance;
                NetInfo info3 = instance3.m_segments.m_buffer[(int)vehicleData.m_targetBuilding].Info;
                if (info3.m_lanes == null)
                {
                    return false;
                }
                int num = 0;
                for (int i = 0; i < info3.m_lanes.Length; i++)
                {
                    if (info3.m_lanes[i].CheckType(NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, VehicleInfo.VehicleType.Car))
                    {
                        num++;
                    }
                }
                if (num == 0)
                {
                    return false;
                }
                Vector3 endPos3 = Vector3.zero;
                num = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint)num);
                uint num2 = instance3.m_segments.m_buffer[(int)vehicleData.m_targetBuilding].m_lanes;
                int num3 = 0;
                while (num3 < info3.m_lanes.Length && num2 != 0u)
                {
                    if (info3.m_lanes[num3].CheckType(NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, VehicleInfo.VehicleType.Car) && num-- == 0)
                    {
                        endPos3 = instance3.m_lanes.m_buffer[(int)((UIntPtr)num2)].CalculatePosition(0.5f);
                        break;
                    }
                    num2 = instance3.m_lanes.m_buffer[(int)((UIntPtr)num2)].m_nextLane;
                    num3++;
                }
                return this.StartPathFind(ExtendedVehicleType.SnowTruck, vehicleID, ref vehicleData, vehicleData.m_targetPos3, endPos3, true, true, true, IsHeavyVehicle(), IgnoreBlocked(vehicleID, ref vehicleData));
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(SnowTruckAI))]
        private bool CheckTargetSegment(ushort vehicleID, ref Vehicle vehicleData)
        {
            throw new NotImplementedException("CheckTargetSegment is target of redirection and is not implemented.");
        }
    }
}
