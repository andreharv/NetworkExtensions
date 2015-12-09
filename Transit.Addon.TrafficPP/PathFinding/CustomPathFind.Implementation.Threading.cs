using System;
using System.Threading;
using ColossalFramework;
using ColossalFramework.UI;

namespace Transit.Addon.TrafficPP.PathFinding
{
	partial class CustomPathFind : PathFind
	{
		private void PathFindThread()
		{
			while (true)
			{
				while (!Monitor.TryEnter(m_queueLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
				{
				}
				try
				{
					while (m_queueFirst == 0u && !m_terminated)
					{
						Monitor.Wait(m_queueLock);
					}
					if (m_terminated)
					{
						break;
					}
					m_calculating = m_queueFirst;
					m_queueFirst = m_pathUnits.m_buffer[(int)((UIntPtr)m_calculating)].m_nextPathUnit;
					if (m_queueFirst == 0u)
					{
						m_queueLast = 0u;
						m_queuedPathFindCount = 0;
					}
					else
					{
						m_queuedPathFindCount--;
					}
					m_pathUnits.m_buffer[(int)((UIntPtr)m_calculating)].m_nextPathUnit = 0u;
					m_pathUnits.m_buffer[(int)((UIntPtr)m_calculating)].m_pathFindFlags = (byte)(((int)m_pathUnits.m_buffer[(int)((UIntPtr)m_calculating)].m_pathFindFlags & -2) | 2);
				}
				finally
				{
					Monitor.Exit(m_queueLock);
				}
				try
				{
					m_pathfindProfiler.BeginStep();
					try
					{
						PathFindImplementation(m_calculating, ref m_pathUnits.m_buffer[(int)((UIntPtr)m_calculating)]);
					}
					finally
					{
						m_pathfindProfiler.EndStep();
					}
				}
				catch (Exception ex)
				{
					UIView.ForwardException(ex);
					CODebugBase<LogChannel>.Error(LogChannel.Core, "Path find error: " + ex.Message/* + " - " + m_vehicleType + " - " + m_vehicleTypes*/ + "\n" + ex.StackTrace);
					PathUnit[] expr_1A0_cp_0 = m_pathUnits.m_buffer;
					UIntPtr expr_1A0_cp_1 = (UIntPtr)m_calculating;
					expr_1A0_cp_0[(int)expr_1A0_cp_1].m_pathFindFlags = (byte)(expr_1A0_cp_0[(int)expr_1A0_cp_1].m_pathFindFlags | 8);
				}
				while (!Monitor.TryEnter(m_queueLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
				{
				}
				try
				{
					m_pathUnits.m_buffer[(int)((UIntPtr)m_calculating)].m_pathFindFlags = (byte)((int)m_pathUnits.m_buffer[(int)((UIntPtr)m_calculating)].m_pathFindFlags & -3);
					Singleton<PathManager>.instance.ReleasePath(m_calculating);
					m_calculating = 0u;
					Monitor.Pulse(m_queueLock);
				}
				finally
				{
					Monitor.Exit(m_queueLock);
				}
			}
		}
	}
}
