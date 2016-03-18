using ColossalFramework;
using Transit.Framework.Redirection;
using UnityEngine;

namespace Transit.Addon.TM.AI
{
    public class TPPPassengerCarAI : CarAI
    {
        [RedirectFrom(typeof(PassengerCarAI))]
        public override void SimulationStep(ushort vehicleID, ref Vehicle data, Vector3 physicsLodRefPos)
        {
            if (((data.m_flags & Vehicle.Flags.Congestion) != Vehicle.Flags.None) &&
                ((ToolModuleV2.ActiveOptions & Options.NoDespawn) != Options.NoDespawn))
            {
                Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleID);
            }
            else
            {
                base.SimulationStep(vehicleID, ref data, physicsLodRefPos);
            }
        }
    }
}
