using ColossalFramework;
using System;
using System.Runtime.CompilerServices;
using ColossalFramework.Math;
using UnityEngine;
using Transit.Framework.Light;
using Transit.Framework.Unsafe;

namespace CSL_Traffic
{
	public class CustomPassengerCarAI : CarAI
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectFrom(typeof(PassengerCarAI))]
        public override void SimulationStep(ushort vehicleID, ref Vehicle data, Vector3 physicsLodRefPos)
		{
			if ((TrafficMod.Options & OptionsManager.ModOptions.NoDespawn) == OptionsManager.ModOptions.NoDespawn)
				data.m_flags &= ~Vehicle.Flags.Congestion;

            if ((data.m_flags & Vehicle.Flags.Congestion) != Vehicle.Flags.None)
            {
                Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleID);
            }
            else
            {
                base.SimulationStep(vehicleID, ref data, physicsLodRefPos);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectFrom(typeof(PassengerCarAI))]
        public override void SimulationStep(ushort vehicleID, ref Vehicle vehicleData, ref Vehicle.Frame frameData, ushort leaderID, ref Vehicle leaderData, int lodPhysics)
		{
			if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
            {
                var speedData = CarSpeedData.Of(vehicleID);

				if (speedData.SpeedMultiplier == 0 || speedData.CurrentPath != vehicleData.m_path)
				{
					speedData.CurrentPath = vehicleData.m_path;
					speedData.SetRandomSpeedMultiplier(0.6f, 1.4f);
				}
				m_info.ApplySpeedMultiplier(CarSpeedData.Of(vehicleID));
            }

            if ((vehicleData.m_flags & Vehicle.Flags.Stopped) != Vehicle.Flags.None)
            {
                vehicleData.m_waitCounter += 1;
                if (this.CanLeave(vehicleID, ref vehicleData))
                {
                    vehicleData.m_flags &= ~Vehicle.Flags.Stopped;
                    vehicleData.m_waitCounter = 0;
                }
            }
            base.SimulationStep(vehicleID, ref vehicleData, ref frameData, leaderID, ref leaderData, lodPhysics);

            if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
			{
				m_info.RestoreVehicleSpeed(CarSpeedData.Of(vehicleID));
			}
		}

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectFrom(typeof(PassengerCarAI))]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData)
        {
            ushort driverInstance = this.GetDriverInstance(vehicleID, ref vehicleData);
            if (driverInstance != 0)
            {
                ushort targetBuilding = Singleton<CitizenManager>.instance.m_instances.m_buffer[(int)driverInstance].m_targetBuilding;
                if (targetBuilding != 0)
                {
                    BuildingManager instance = Singleton<BuildingManager>.instance;
                    BuildingInfo info = instance.m_buildings.m_buffer[(int)targetBuilding].Info;
                    Randomizer randomizer = new Randomizer((int)vehicleID);
                    Vector3 vector;
                    Vector3 endPos;
                    info.m_buildingAI.CalculateUnspawnPosition(targetBuilding, ref instance.m_buildings.m_buffer[(int)targetBuilding], ref randomizer, this.m_info, out vector, out endPos);
                    return this.StartPathFind(ExtendedVehicleType.PassengerCar, vehicleID, ref vehicleData, vehicleData.m_targetPos3, endPos, IsHeavyVehicle(), IgnoreBlocked(vehicleID, ref vehicleData));
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
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
            if (CustomPathManager.FindPathPosition(ExtendedVehicleType.PassengerCar, startPos, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, info.m_vehicleType, allowUnderground, false, 32f, out startPosA, out startPosB, out num, out num2) && info2.m_citizenAI.FindPathPosition(driverInstance, ref instance.m_instances.m_buffer[(int)driverInstance], endPos, laneTypes, vehicleType, undergroundTarget, out endPosA))
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
        [RedirectFrom(typeof(PassengerCarAI))]
        private static bool FindParkingSpaceRoadSide(ushort ignoreParked, ushort requireSegment, Vector3 refPos, float width, float length, out Vector3 parkPos, out Quaternion parkRot, out float parkOffset)
        {
            parkPos = Vector3.zero;
            parkRot = Quaternion.identity;
            parkOffset = 0f;
            PathUnit.Position pathPos;
            if (CustomPathManager.FindPathPosition(ExtendedVehicleType.PassengerCar, refPos, ItemClass.Service.Road, NetInfo.LaneType.Parking, VehicleInfo.VehicleType.Car, false, false, 32f, out pathPos))
            {
                if (requireSegment != 0 && pathPos.m_segment != requireSegment)
                {
                    return false;
                }
                NetManager instance = Singleton<NetManager>.instance;
                NetInfo info = instance.m_segments.m_buffer[(int)pathPos.m_segment].Info;
                uint laneID = PathManager.GetLaneID(pathPos);
                uint num = instance.m_segments.m_buffer[(int)pathPos.m_segment].m_lanes;
                int num2 = 0;
                while (num2 < info.m_lanes.Length && num != 0u)
                {
                    if ((instance.m_lanes.m_buffer[(int)((UIntPtr)num)].m_flags & 768) != 0 && info.m_lanes[(int)pathPos.m_lane].m_position >= 0f == info.m_lanes[num2].m_position >= 0f)
                    {
                        return false;
                    }
                    num = instance.m_lanes.m_buffer[(int)((UIntPtr)num)].m_nextLane;
                    num2++;
                }
                bool flag = (instance.m_segments.m_buffer[(int)pathPos.m_segment].m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None;
                bool flag2 = (byte)(info.m_lanes[(int)pathPos.m_lane].m_finalDirection & NetInfo.Direction.Forward) == 0;
                bool flag3 = info.m_lanes[(int)pathPos.m_lane].m_position < 0f;
                float num3 = (float)pathPos.m_offset * 0.003921569f;
                float num4;
                float num5;
                if (CheckOverlap(ignoreParked, ref instance.m_lanes.m_buffer[(int)((UIntPtr)laneID)].m_bezier, num3, length, out num4, out num5))
                {
                    num3 = -1f;
                    for (int i = 0; i < 6; i++)
                    {
                        if (num5 <= 1f)
                        {
                            float num6;
                            float num7;
                            if (!CheckOverlap(ignoreParked, ref instance.m_lanes.m_buffer[(int)((UIntPtr)laneID)].m_bezier, num5, length, out num6, out num7))
                            {
                                num3 = num5;
                                break;
                            }
                            num5 = num7;
                        }
                        if (num4 >= 0f)
                        {
                            float num6;
                            float num7;
                            if (!CheckOverlap(ignoreParked, ref instance.m_lanes.m_buffer[(int)((UIntPtr)laneID)].m_bezier, num4, length, out num6, out num7))
                            {
                                num3 = num4;
                                break;
                            }
                            num4 = num6;
                        }
                    }
                }
                if (num3 >= 0f)
                {
                    Vector3 vector;
                    Vector3 vector2;
                    instance.m_lanes.m_buffer[(int)((UIntPtr)laneID)].CalculatePositionAndDirection(num3, out vector, out vector2);
                    float num8 = (info.m_lanes[(int)pathPos.m_lane].m_width - width) * 0.5f;
                    vector2.Normalize();
                    if (flag != flag3)
                    {
                        parkPos.x = vector.x - vector2.z * num8;
                        parkPos.y = vector.y;
                        parkPos.z = vector.z + vector2.x * num8;
                    }
                    else
                    {
                        parkPos.x = vector.x + vector2.z * num8;
                        parkPos.y = vector.y;
                        parkPos.z = vector.z - vector2.x * num8;
                    }
                    if (flag != flag2)
                    {
                        parkRot = Quaternion.LookRotation(-vector2);
                    }
                    else
                    {
                        parkRot = Quaternion.LookRotation(vector2);
                    }
                    parkOffset = num3;
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

	    [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(PassengerCarAI))]
        private static bool CheckOverlap(ushort ignoreParked, ref Bezier3 bezier, float offset, float length, out float minPos, out float maxPos)
        {
            throw new NotImplementedException("CheckOverlap is target of redirection and is not implemented.");
        }
    }
}
