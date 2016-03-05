using System;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.Math;
using Transit.Framework.Network;
using UnityEngine;
using Transit.Framework.Redirection;

namespace CSL_Traffic
{
    public class CustomHearseAI : CarAI
    {
        [RedirectFrom(typeof(HearseAI))]
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
                    return this.StartPathFind(ExtendedVehicleType.Hearse, vehicleID, ref vehicleData, vehicleData.m_targetPos3, endPos, IsHeavyVehicle(), IgnoreBlocked(vehicleID, ref vehicleData));
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
                return this.StartPathFind(ExtendedVehicleType.Hearse, vehicleID, ref vehicleData, vehicleData.m_targetPos3, endPos2, IsHeavyVehicle(), IgnoreBlocked(vehicleID, ref vehicleData));
            }
            return false;
        }
    }
}
