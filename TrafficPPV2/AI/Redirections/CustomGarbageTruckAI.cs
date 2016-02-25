using ColossalFramework;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Transit.Framework.Light;
using Transit.Framework.Unsafe;

namespace CSL_Traffic
{
    public class CustomGarbageTruckAI : CarAI
    {
        [RedirectFrom(typeof(GarbageTruckAI))]
        public override void SimulationStep(ushort vehicleID, ref Vehicle vehicleData, ref Vehicle.Frame frameData, ushort leaderID, ref Vehicle leaderData, int lodPhysics)
        {
            if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
            {
                var speedData = CarSpeedData.Of(vehicleID);

                if (speedData.SpeedMultiplier == 0 || speedData.CurrentPath != vehicleData.m_path)
                {
                    speedData.CurrentPath = vehicleData.m_path;
                    speedData.SetRandomSpeedMultiplier(0.75f, 0.95f);
                }
                m_info.ApplySpeedMultiplier(CarSpeedData.Of(vehicleID));
            }

            if ((vehicleData.m_flags & Vehicle.Flags.TransferToSource) != Vehicle.Flags.None)
            {
                var self = (GarbageTruckAI)((object)this);
                if ((int)vehicleData.m_transferSize < self.m_cargoCapacity)
                {
                    this.TryCollectGarbage(vehicleID, ref vehicleData, ref frameData);
                }
                if ((int)vehicleData.m_transferSize >= self.m_cargoCapacity && (vehicleData.m_flags & Vehicle.Flags.GoingBack) == Vehicle.Flags.None && vehicleData.m_targetBuilding != 0)
                {
                    this.SetTarget(vehicleID, ref vehicleData, 0);
                }
            }
            base.SimulationStep(vehicleID, ref vehicleData, ref frameData, leaderID, ref leaderData, lodPhysics);
            if ((vehicleData.m_flags & Vehicle.Flags.Arriving) != Vehicle.Flags.None && vehicleData.m_targetBuilding != 0 && (vehicleData.m_flags & (Vehicle.Flags.WaitingPath | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget)) == Vehicle.Flags.None)
            {
                this.ArriveAtTarget(vehicleID, ref vehicleData);
            }
            if ((vehicleData.m_flags & (Vehicle.Flags.TransferToSource | Vehicle.Flags.GoingBack)) == Vehicle.Flags.TransferToSource && this.ShouldReturnToSource(vehicleID, ref vehicleData))
            {
                this.SetTarget(vehicleID, ref vehicleData, 0);
            }

            if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
            {
                m_info.RestoreVehicleSpeed(CarSpeedData.Of(vehicleID));
            }
        }

        [RedirectFrom(typeof(GarbageTruckAI))]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData, Vector3 startPos, Vector3 endPos, bool startBothWays, bool endBothWays, bool undergroundTarget)
        {
            return this.StartPathFind(ExtendedVehicleType.GarbageTruck, vehicleID, ref vehicleData, startPos, endPos, startBothWays, endBothWays, undergroundTarget, IsHeavyVehicle(), IgnoreBlocked(vehicleID, ref vehicleData));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GarbageTruckAI))]
        private void TryCollectGarbage(ushort vehicleID, ref Vehicle vehicleData, ref Vehicle.Frame frameData)
        {
            throw new NotImplementedException("TryCollectGarbage is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GarbageTruckAI))]
        private bool ArriveAtTarget(ushort vehicleID, ref Vehicle data)
        {
            throw new NotImplementedException("ArriveAtTarget is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(GarbageTruckAI))]
        private bool ShouldReturnToSource(ushort vehicleID, ref Vehicle data)
        {
            throw new NotImplementedException("ShouldReturnToSource is target of redirection and is not implemented.");
        }
    }
}
