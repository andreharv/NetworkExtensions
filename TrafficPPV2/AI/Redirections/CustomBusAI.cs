using System;
using System.Runtime.CompilerServices;
using ColossalFramework;
using Transit.Framework.Network;
using UnityEngine;
using Transit.Framework.Redirection;

namespace CSL_Traffic
{
    public class CustomBusAI : CarAI
    {
        [RedirectFrom(typeof(BusAI))]
        public override void SimulationStep(ushort vehicleID, ref Vehicle vehicleData, ref Vehicle.Frame frameData, ushort leaderID, ref Vehicle leaderData, int lodPhysics)
        {
            if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
            {
                var speedData = CarSpeedData.Of(vehicleID);

                if (speedData.SpeedMultiplier == 0 || speedData.CurrentPath != vehicleData.m_path)
                {
                    speedData.CurrentPath = vehicleData.m_path;
                    speedData.SetRandomSpeedMultiplier(0.65f, 1.05f);
                }
                m_info.ApplySpeedMultiplier(CarSpeedData.Of(vehicleID));
            }


            if ((vehicleData.m_flags & Vehicle.Flags.Stopped) != 0)
            {
                vehicleData.m_waitCounter += 1;
                if (this.CanLeave(vehicleID, ref vehicleData))
                {
                    vehicleData.m_flags &= ~Vehicle.Flags.Stopped;
                    vehicleData.m_flags |= Vehicle.Flags.Leaving;
                    vehicleData.m_waitCounter = 0;
                }
            }
            base.SimulationStep(vehicleID, ref vehicleData, ref frameData, leaderID, ref leaderData, lodPhysics);
            if ((vehicleData.m_flags & Vehicle.Flags.GoingBack) == 0 && this.ShouldReturnToSource(vehicleID, ref vehicleData))
            {
                this.SetTransportLine(vehicleID, ref vehicleData, 0);
            }

            if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
            {
                m_info.RestoreVehicleSpeed(CarSpeedData.Of(vehicleID));
            }
        }

        [RedirectFrom(typeof(BusAI))]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData, Vector3 startPos, Vector3 endPos, bool startBothWays, bool endBothWays, bool undergroundTarget)
        {
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
                bool createPathResult = Singleton<PathManager>.instance.CreatePath(ExtendedVehicleType.Bus, out path, ref Singleton<SimulationManager>.instance.m_randomizer, Singleton<SimulationManager>.instance.m_currentBuildIndex, startPosA, startPosB, endPosA, endPosB, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, info.m_vehicleType, 20000f, this.IsHeavyVehicle(), this.IgnoreBlocked(vehicleID, ref vehicleData), false, false);
                if (createPathResult)
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
        [RedirectTo(typeof(BusAI))]
        private bool ShouldReturnToSource(ushort vehicleID, ref Vehicle data)
        {
            throw new NotImplementedException("ShouldReturnToSource is target of redirection and is not implemented.");
        }
    }
}
