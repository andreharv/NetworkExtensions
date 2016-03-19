using Transit.Framework.ExtensionPoints.AI.Units;
using Transit.Framework.Network;
using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;
using UnityEngine;

namespace Transit.Framework.Hooks.AI.Units
{
    public class MaintenanceTruckAIHook : MaintenanceTruckAI
    {
        [RedirectFrom(typeof(MaintenanceTruckAI), (ulong)PrerequisiteType.PathFinding)]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData, Vector3 startPos, Vector3 endPos, bool startBothWays, bool endBothWays, bool undergroundTarget)
        {
            return this.StartPathFind(ExtendedUnitType.MaintenanceTruck, vehicleID, ref vehicleData, startPos, endPos, startBothWays, endBothWays, undergroundTarget, IsHeavyVehicle(), IgnoreBlocked(vehicleID, ref vehicleData));
        }
    }
}
