using ColossalFramework;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.Network;
using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;
using UnityEngine;

namespace Transit.Framework.Hooks.AI.Units
{
    public class FireTruckAIHook : CarAI
    {
        [RedirectFrom(typeof(FireTruckAI), (ulong)PrerequisiteType.PathFinding)]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData, Vector3 startPos, Vector3 endPos, bool startBothWays, bool endBothWays, bool undergroundTarget)
        {
            ExtendedUnitType vehicleType = ExtendedUnitType.FireTruck;
            if ((vehicleData.m_flags & Vehicle.Flags.Emergency2) != Vehicle.Flags.None)
                vehicleType |= ExtendedUnitType.Emergency;

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
    }
}
