using System;
using ColossalFramework;
using UnityEngine;
using Transit.Addon.TM.Traffic;

namespace Transit.Addon.TM.AI
{
    class CustomPassengerCarAI : CarAI
    {
        public void CustomSimulationStep(ushort vehicleId, ref Vehicle data, Vector3 physicsLodRefPos)
        {
			try {
				if ((data.m_flags & Vehicle.Flags.Congestion) != Vehicle.Flags.None && OptionManager.enableDespawning) {
					Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleId);
				} else {
					base.SimulationStep(vehicleId, ref data, physicsLodRefPos);
				}
			} catch (Exception ex) {
				Log.Error("Error in CustomPassengerCarAI.SimulationStep: " + ex.ToString());
			}
        }

		public static ushort GetDriverInstance(ushort vehicleID, ref Vehicle data) {
			CitizenManager instance = Singleton<CitizenManager>.instance;
			uint num = data.m_citizenUnits;
			int num2 = 0;
			while (num != 0u) {
				uint nextUnit = instance.m_units.m_buffer[(int)((UIntPtr)num)].m_nextUnit;
				for (int i = 0; i < 5; i++) {
					uint citizen = instance.m_units.m_buffer[(int)((UIntPtr)num)].GetCitizen(i);
					if (citizen != 0u) {
						ushort instance2 = instance.m_citizens.m_buffer[(int)((UIntPtr)citizen)].m_instance;
						if (instance2 != 0) {
							return instance2;
						}
					}
				}
				num = nextUnit;
				if (++num2 > 524288) {
					CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
					break;
				}
			}
			return 0;
		}
	}
}
