using ColossalFramework;
using System;
using System.Runtime.CompilerServices;
using Transit.Framework.Network;
using UnityEngine;
using Transit.Framework.Redirection;

namespace CSL_Traffic
{
    public class CustomFireTruckAI : CarAI
    {
        [RedirectFrom(typeof(FireTruckAI))]
        public override void SimulationStep(ushort vehicleID, ref Vehicle vehicleData, ref Vehicle.Frame frameData, ushort leaderID, ref Vehicle leaderData, int lodPhysics)
        {
            if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
            {
                var speedData = CarSpeedData.Of(vehicleID);

                if (speedData.SpeedMultiplier == 0 || speedData.CurrentPath != vehicleData.m_path)
                {
                    speedData.CurrentPath = vehicleData.m_path;
                    if ((vehicleData.m_flags & Vehicle.Flags.Emergency2) == Vehicle.Flags.Emergency2)
                        speedData.SetRandomSpeedMultiplier(1f, 1.75f);
                    else
                        speedData.SetRandomSpeedMultiplier(0.65f, 1f);
                }
                m_info.ApplySpeedMultiplier(CarSpeedData.Of(vehicleID));
            }


            frameData.m_blinkState = (((vehicleData.m_flags & (Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2)) == 0) ? 0f : 10f);
            base.SimulationStep(vehicleID, ref vehicleData, ref frameData, leaderID, ref leaderData, lodPhysics);
            bool flag = false;
            if (vehicleData.m_targetBuilding != 0)
            {
                BuildingManager instance = Singleton<BuildingManager>.instance;
                Vector3 a = instance.m_buildings.m_buffer[(int)vehicleData.m_targetBuilding].CalculateSidewalkPosition();
                flag = ((a - frameData.m_position).sqrMagnitude < 4096f);
                bool flag2 = (vehicleData.m_flags & Vehicle.Flags.Stopped) != 0 || frameData.m_velocity.sqrMagnitude < 0.0100000007f;
                if (flag && (vehicleData.m_flags & Vehicle.Flags.Emergency2) != 0)
                {
                    vehicleData.m_flags = ((vehicleData.m_flags & ~Vehicle.Flags.Emergency2) | Vehicle.Flags.Emergency1);
                }
                if (flag && flag2)
                {
                    if (vehicleData.m_blockCounter > 8)
                    {
                        vehicleData.m_blockCounter = 8;
                    }
                    if (vehicleData.m_blockCounter == 8 && (vehicleData.m_flags & Vehicle.Flags.Stopped) == 0)
                    {
                        this.ArriveAtTarget(leaderID, ref leaderData);
                    }
                    if (this.ExtinguishFire(vehicleID, ref vehicleData, vehicleData.m_targetBuilding, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)vehicleData.m_targetBuilding]))
                    {
                        this.SetTarget(vehicleID, ref vehicleData, 0);
                    }
                }
                else if (instance.m_buildings.m_buffer[(int)vehicleData.m_targetBuilding].m_fireIntensity == 0)
                {
                    this.SetTarget(vehicleID, ref vehicleData, 0);
                }
            }
            if ((vehicleData.m_flags & Vehicle.Flags.Stopped) != 0 && !flag && this.CanLeave(vehicleID, ref vehicleData))
            {
                vehicleData.m_flags &= ~Vehicle.Flags.Stopped;
                vehicleData.m_flags |= Vehicle.Flags.Leaving;
            }
            if ((vehicleData.m_flags & Vehicle.Flags.GoingBack) == 0)
            {
                if (this.ShouldReturnToSource(vehicleID, ref vehicleData))
                {
                    this.SetTarget(vehicleID, ref vehicleData, 0);
                }
            }
            else if ((ulong)(Singleton<SimulationManager>.instance.m_currentFrameIndex >> 4 & 15u) == (ulong)((long)(vehicleID & 15)) && !this.ShouldReturnToSource(vehicleID, ref vehicleData))
            {
                TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                offer.Priority = 3;
                offer.Vehicle = vehicleID;
                offer.Position = frameData.m_position;
                offer.Amount = 1;
                offer.Active = true;
                Singleton<TransferManager>.instance.AddIncomingOffer((TransferManager.TransferReason)vehicleData.m_transferType, offer);
            }

            if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
            {
                m_info.RestoreVehicleSpeed(CarSpeedData.Of(vehicleID));
            }
        }
        
        [RedirectFrom(typeof(FireTruckAI))]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData, Vector3 startPos, Vector3 endPos, bool startBothWays, bool endBothWays, bool undergroundTarget)
        {
            ExtendedVehicleType vehicleType = ExtendedVehicleType.FireTruck;
            if ((vehicleData.m_flags & Vehicle.Flags.Emergency2) != 0)
                vehicleType |= ExtendedVehicleType.Emergency;

            VehicleInfo info = this.m_info;
            bool allowUnderground = (vehicleData.m_flags & (Vehicle.Flags.Underground | Vehicle.Flags.Transition)) != 0;
            PathUnit.Position startPosA;
            PathUnit.Position startPosB;
            float num;
            float num2;
            PathUnit.Position endPosA;
            PathUnit.Position endPosB;
            float num3;
            float num4;
            if (PathManager.FindPathPosition(startPos, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, info.m_vehicleType, allowUnderground, false, 32f, out startPosA, out startPosB, out num, out num2) && PathManager.FindPathPosition(endPos, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, info.m_vehicleType, undergroundTarget, false, 32f, out endPosA, out endPosB, out num3, out num4))
            {
                if (!startBothWays || num < 10f)
                {
                    startPosB = default(PathUnit.Position);
                }
                if (!endBothWays || num3 < 10f)
                {
                    endPosB = default(PathUnit.Position);
                }
                uint path;
                if (Singleton<PathManager>.instance.CreatePath(vehicleType, out path, ref Singleton<SimulationManager>.instance.m_randomizer, Singleton<SimulationManager>.instance.m_currentBuildIndex, startPosA, startPosB, endPosA, endPosB, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, info.m_vehicleType, 20000f, this.IsHeavyVehicle(), this.IgnoreBlocked(vehicleID, ref vehicleData), false, false))
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
        [RedirectTo(typeof(FireTruckAI))]
        private bool ArriveAtTarget(ushort vehicleID, ref Vehicle data)
        {
            throw new NotImplementedException("ArriveAtTarget is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(FireTruckAI))]
        private bool ExtinguishFire(ushort vehicleID, ref Vehicle data, ushort buildingID, ref Building buildingData)
        {
            throw new NotImplementedException("ExtinguishFire is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(FireTruckAI))]
        private bool ShouldReturnToSource(ushort vehicleID, ref Vehicle data)
        {
            throw new NotImplementedException("ShouldReturnToSource is target of redirection and is not implemented.");
        }
    }
}
