using ColossalFramework;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Transit.Framework.Network;
using UnityEngine;
using Transit.Framework.Redirection;

namespace CSL_Traffic
{
    public class CustomGarbageTruckAI : CarAI
    {
        [RedirectFrom(typeof(GarbageTruckAI))]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData, Vector3 startPos, Vector3 endPos, bool startBothWays, bool endBothWays, bool undergroundTarget)
        {
            return this.StartPathFind(ExtendedVehicleType.GarbageTruck, vehicleID, ref vehicleData, startPos, endPos, startBothWays, endBothWays, undergroundTarget, IsHeavyVehicle(), IgnoreBlocked(vehicleID, ref vehicleData));
        }
    }
}
