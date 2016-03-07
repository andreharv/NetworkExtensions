using ColossalFramework;
using System;
using System.Runtime.CompilerServices;
using ColossalFramework.Math;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.Network;
using UnityEngine;
using Transit.Framework.Redirection;

namespace CSL_Traffic
{
	public class CustomPassengerCarAI : CarAI
    {
        [RedirectFrom(typeof(PassengerCarAI))]
        public override void SimulationStep(ushort vehicleID, ref Vehicle data, Vector3 physicsLodRefPos)
        {
            if (((data.m_flags & Vehicle.Flags.Congestion) != Vehicle.Flags.None) &&
                ((Mod.Options & ModOptions.NoDespawn) != ModOptions.NoDespawn))
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
