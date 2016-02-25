using System;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.Math;
using Transit.Framework.Redirection;
using UnityEngine;

namespace CSL_Traffic
{
    public class CustomAmbulanceAI : CarAI
    {
        [RedirectFrom(typeof(AmbulanceAI))]
        public override void SimulationStep(ushort vehicleID, ref Vehicle vehicleData, ref Vehicle.Frame frameData, ushort leaderID, ref Vehicle leaderData, int lodPhysics)
        {
            if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
            {
                var speedData = CarSpeedData.Of(vehicleID);

                if (speedData.SpeedMultiplier == 0 || speedData.CurrentPath != vehicleData.m_path)
                {
                    speedData.CurrentPath = vehicleData.m_path;
                    if ((vehicleData.m_flags & Vehicle.Flags.Emergency2) == Vehicle.Flags.Emergency2)
                        speedData.SetRandomSpeedMultiplier(1f, 1.5f);
                    else
                        speedData.SetRandomSpeedMultiplier(0.7f, 1.05f);
                }
                m_info.ApplySpeedMultiplier(speedData);
            }


            frameData.m_blinkState = (((vehicleData.m_flags & Vehicle.Flags.Emergency2) == Vehicle.Flags.None) ? 0f : 10f);
            base.SimulationStep(vehicleID, ref vehicleData, ref frameData, leaderID, ref leaderData, lodPhysics);
            if ((vehicleData.m_flags & Vehicle.Flags.Stopped) != Vehicle.Flags.None && this.CanLeave(vehicleID, ref vehicleData))
            {
                vehicleData.m_flags &= ~Vehicle.Flags.Stopped;
                vehicleData.m_flags |= Vehicle.Flags.Leaving;
            }
            if ((vehicleData.m_flags & Vehicle.Flags.GoingBack) == Vehicle.Flags.None && this.ShouldReturnToSource(vehicleID, ref vehicleData))
            {
                this.SetTarget(vehicleID, ref vehicleData, 0);
            }

            if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
            {
                m_info.RestoreVehicleSpeed(CarSpeedData.Of(vehicleID));
            }
        }

        [RedirectFrom(typeof(AmbulanceAI))]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData)
        {
            Transit.Framework.Light.ExtendedVehicleType vehicleType = Transit.Framework.Light.ExtendedVehicleType.Ambulance;
            if ((vehicleData.m_flags & Vehicle.Flags.Emergency2) != Vehicle.Flags.None)
                vehicleType |= Transit.Framework.Light.ExtendedVehicleType.Emergency;

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
                    return this.StartPathFind(vehicleType, vehicleID, ref vehicleData, vehicleData.m_targetPos3, endPos, IsHeavyVehicle(), IgnoreBlocked(vehicleID, ref vehicleData));
                }
            }
            else if (vehicleData.m_targetBuilding != 0)
            {
                BuildingManager instance2 = Singleton<BuildingManager>.instance;
                BuildingInfo info2 = instance2.m_buildings.m_buffer[(int)vehicleData.m_targetBuilding].Info;
                Randomizer randomizer2 = new Randomizer((int)vehicleID);
                Vector3 vector2;
                Vector3 endPos2;
                info2.m_buildingAI.CalculateUnspawnPosition(vehicleData.m_targetBuilding, ref instance2.m_buildings.m_buffer[(int)vehicleData.m_targetBuilding], ref randomizer2, this.m_info, out vector2, out endPos2);
                return this.StartPathFind(vehicleType, vehicleID, ref vehicleData, vehicleData.m_targetPos3, endPos2, IsHeavyVehicle(), IgnoreBlocked(vehicleID, ref vehicleData));
            }
            return false;
        }

        [RedirectFrom(typeof(AmbulanceAI))]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData, Vector3 startPos, Vector3 endPos, bool startBothWays, bool endBothWays, bool undergroundTarget)
        {
            Transit.Framework.Light.ExtendedVehicleType vehicleType = Transit.Framework.Light.ExtendedVehicleType.Ambulance;
            if ((vehicleData.m_flags & Vehicle.Flags.Emergency2) != Vehicle.Flags.None)
                vehicleType |= Transit.Framework.Light.ExtendedVehicleType.Emergency;

            VehicleInfo info = this.m_info;
            bool allowUnderground = (vehicleData.m_flags & (Vehicle.Flags.Underground | Vehicle.Flags.Transition)) != Vehicle.Flags.None;
            PathUnit.Position startPosA;
            PathUnit.Position startPosB;
            float num;
            float num2;
            PathUnit.Position endPosA;
            PathUnit.Position endPosB;
            float num3;
            float num4;
            if (CustomPathManager.FindPathPosition(vehicleType, startPos, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, info.m_vehicleType, allowUnderground, false, 32f, out startPosA, out startPosB, out num, out num2) && CustomPathManager.FindPathPosition(vehicleType, endPos, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, info.m_vehicleType, undergroundTarget, false, 32f, out endPosA, out endPosB, out num3, out num4))
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
                bool createPathResult = Singleton<PathManager>.instance.CreatePath(vehicleType, out path, ref Singleton<SimulationManager>.instance.m_randomizer, Singleton<SimulationManager>.instance.m_currentBuildIndex, startPosA, startPosB, endPosA, endPosB, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, info.m_vehicleType, 20000f, this.IsHeavyVehicle(), this.IgnoreBlocked(vehicleID, ref vehicleData), false, false);
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
        [RedirectTo(typeof(AmbulanceAI))]
        private bool ShouldReturnToSource(ushort vehicleID, ref Vehicle data)
        {
            throw new NotImplementedException("ShouldReturnToSource is target of redirection and is not implemented.");
        }
    }
}
