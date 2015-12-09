using System;
using System.Threading;
using ColossalFramework;
using Transit.Addon.TrafficPP.Core;

namespace Transit.Addon.TrafficPP.PathFinding
{
	partial class CustomPathFind : PathFind
	{
		public bool CalculatePath(uint unit, bool skipQueue, VehicleTypePP vehicleType)
		{
			if (Singleton<PathManager>.instance.AddPathReference(unit))
			{
				while (!Monitor.TryEnter(m_queueLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
				{
				}
				try
				{
					if (skipQueue)
					{
						if (m_queueLast == 0u)
						{
							m_queueLast = unit;
						}
						else
						{
							m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_nextPathUnit = m_queueFirst;
						}
						m_queueFirst = unit;
					}
					else
					{
						if (m_queueLast == 0u)
						{
							m_queueFirst = unit;
						}
						else
						{
							m_pathUnits.m_buffer[(int)((UIntPtr)m_queueLast)].m_nextPathUnit = unit;
						}
						m_queueLast = unit;
					}

					m_pathVehicleType[unit] = vehicleType;

					var pathBuffer = m_pathUnits.m_buffer;
					var unitPtr = (UIntPtr)unit;
					pathBuffer[(int)unitPtr].m_pathFindFlags = (byte)(pathBuffer[(int)unitPtr].m_pathFindFlags | 1);
					m_queuedPathFindCount++;
					Monitor.Pulse(m_queueLock);
				}
				finally
				{
					Monitor.Exit(m_queueLock);
				}
				return true;
			}
			return false;
		}

		//public void WaitForAllPaths()
		//{
		//    while (!Monitor.TryEnter(m_queueLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
		//    {
		//    }
		//    try
		//    {
		//        while ((m_queueFirst != 0u || m_calculating != 0u) && !m_terminated)
		//        {
		//            Monitor.Wait(m_queueLock);
		//        }
		//    }
		//    finally
		//    {
		//        Monitor.Exit(m_queueLock);
		//    }
		//}
	}
}
