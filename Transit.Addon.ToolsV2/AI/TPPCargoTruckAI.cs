using System;
using System.Runtime.CompilerServices;
using ColossalFramework;
using Transit.Framework.Redirection;
using UnityEngine;

namespace Transit.Addon.ToolsV2.AI
{
    public class TPPCargoTruckAI : CarAI
    {
        [RedirectFrom(typeof(CargoTruckAI))]
        public override void SimulationStep(ushort vehicleID, ref Vehicle data, Vector3 physicsLodRefPos)
        {
            if (((data.m_flags & Vehicle.Flags.Congestion) != Vehicle.Flags.None) &&
                ((ToolModuleV2.ActiveOptions & ModOptions.NoDespawn) != ModOptions.NoDespawn))
            {
                Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleID);
            }
            else
            {
                if ((data.m_flags & Vehicle.Flags.WaitingTarget) != Vehicle.Flags.None && (data.m_waitCounter += 1) > 20)
                {
                    this.RemoveOffers(vehicleID, ref data);
                    data.m_flags &= ~Vehicle.Flags.WaitingTarget;
                    data.m_flags |= Vehicle.Flags.GoingBack;
                    data.m_waitCounter = 0;
                    if (!this.StartPathFind(vehicleID, ref data))
                    {
                        data.Unspawn(vehicleID);
                    }
                }
                base.SimulationStep(vehicleID, ref data, physicsLodRefPos);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(CargoTruckAI))]
        private void RemoveOffers(ushort vehicleID, ref Vehicle data)
        {
            throw new NotImplementedException("RemoveOffers is target of redirection and is not implemented.");
        }
    }
}
