using System;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.Math;
using Transit.Framework.Network;
using Transit.Framework.Redirection;
using UnityEngine;

namespace CSL_Traffic
{
    public static class CustomCitizenAIExtensions
    {
        public static bool FindPathPosition(this CitizenAI citizenAI, ExtendedVehicleType extendedVehicleType, ushort instanceID, ref CitizenInstance citizenData, Vector3 pos, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, bool allowUnderground, out PathUnit.Position position)
        {
            position = default(PathUnit.Position);
            float num = 1E+10f;
            PathUnit.Position position2;
            PathUnit.Position position3;
            float num2;
            float num3;
            if (ExtendedPathManager.FindPathPosition(extendedVehicleType, pos, ItemClass.Service.Road, laneTypes, vehicleTypes, allowUnderground, false, 32f, out position2, out position3, out num2, out num3) && num2 < num)
            {
                num = num2;
                position = position2;
            }
            PathUnit.Position position4;
            PathUnit.Position position5;
            float num4;
            float num5;
            if (ExtendedPathManager.FindPathPosition(extendedVehicleType, pos, ItemClass.Service.Beautification, laneTypes, vehicleTypes, allowUnderground, false, 32f, out position4, out position5, out num4, out num5) && num4 < num)
            {
                num = num4;
                position = position4;
            }
            PathUnit.Position position6;
            PathUnit.Position position7;
            float num6;
            float num7;
            if ((citizenData.m_flags & CitizenInstance.Flags.CannotUseTransport) == CitizenInstance.Flags.None && ExtendedPathManager.FindPathPosition(extendedVehicleType, pos, ItemClass.Service.PublicTransport, laneTypes, vehicleTypes, allowUnderground, false, 32f, out position6, out position7, out num6, out num7) && num6 < num)
            {
                position = position6;
            }
            return position.m_segment != 0;
        }
    }
}
