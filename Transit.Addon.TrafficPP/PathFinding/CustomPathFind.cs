using ColossalFramework;
using ColossalFramework.Math;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Transit.Addon.TrafficPP.Core;
using Transit.Framework;

namespace Transit.Addon.TrafficPP.PathFinding
{
	/*
	 * This is the class responsible for pathfinding. It's all in here since none of the methods can be overwritten.
	 * There's a lot of small changes here and there to make it generate a correct path for the service vehicles using pedestrian paths.
	 */
	partial class CustomPathFind : PathFind
	{
		FieldInfo fi_pathUnits;
		private Array32<PathUnit> m_pathUnits
		{
			get { return (Array32<PathUnit>)fi_pathUnits.GetValue(this); }
			set { fi_pathUnits.SetValue(this, value); }
		}
		FieldInfo fi_queueFirst;
		private uint m_queueFirst
		{
			get { return (uint)fi_queueFirst.GetValue(this); }
			set { fi_queueFirst.SetValue(this, value); }
		}
		FieldInfo fi_queueLast;
		private uint m_queueLast
		{
			get { return (uint)fi_queueLast.GetValue(this); }
			set { fi_queueLast.SetValue(this, value); }
		}
		FieldInfo fi_calculating;
		private uint m_calculating
		{
			get { return (uint)fi_calculating.GetValue(this); }
			set { fi_calculating.SetValue(this, value); }
		}
		FieldInfo fi_queueLock;
		private object m_queueLock
		{
			get { return fi_queueLock.GetValue(this); }
			set { fi_queueLock.SetValue(this, value); }
		}
		private object m_bufferLock;
		FieldInfo fi_pathFindThread;
		private Thread m_pathFindThread
		{
			get { return (Thread)fi_pathFindThread.GetValue(this); }
			set { fi_pathFindThread.SetValue(this, value); }
		}
		FieldInfo fi_terminated;
		private bool m_terminated
		{
			get { return (bool)fi_terminated.GetValue(this); }
			set { fi_terminated.SetValue(this, value); }
		}

		private int m_bufferMinPos;
		private int m_bufferMaxPos;
		private uint[] m_laneLocation;
		private PathUnit.Position[] m_laneTarget;
		private int[] m_bufferMin;
		private int[] m_bufferMax;
		private float m_maxLength;
		private uint m_startLaneA;
		private uint m_startLaneB;
		private uint m_endLaneA;
		private uint m_endLaneB;
		private uint m_vehicleLane;
		private byte m_startOffsetA;
		private byte m_startOffsetB;
		private byte m_vehicleOffset;
		private bool m_isHeavyVehicle;
		private bool m_ignoreBlocked;
		private bool m_stablePath;
        private bool m_transportVehicle;
		private Randomizer m_pathRandomizer;
		private uint m_pathFindIndex;
		private NetInfo.LaneType m_laneTypes;
		private VehicleInfo.VehicleType m_vehicleTypes;
		private VehicleTypePP m_vehicleType;
		private Dictionary<uint, VehicleTypePP> m_pathVehicleType;
		private bool m_prioritizeBusLanes;

		private void Awake()
		{
			Type pathFindType = typeof(PathFind);
			fi_pathUnits = pathFindType.GetFieldByName("m_pathUnits");
			fi_queueFirst = pathFindType.GetFieldByName("m_queueFirst");
			fi_queueLast = pathFindType.GetFieldByName("m_queueLast");
			fi_calculating = pathFindType.GetFieldByName("m_calculating");
			fi_queueLock = pathFindType.GetFieldByName("m_queueLock");
			fi_pathFindThread = pathFindType.GetFieldByName("m_pathFindThread");
			fi_terminated = pathFindType.GetFieldByName("m_terminated");

			this.m_pathfindProfiler = new ThreadProfiler();
			this.m_laneLocation = new uint[262144];
			this.m_laneTarget = new PathUnit.Position[262144];
			this.m_buffer = new BufferItem[65536];
			this.m_bufferMin = new int[1024];
			this.m_bufferMax = new int[1024];
			this.m_queueLock = new object();
			this.m_pathVehicleType = new Dictionary<uint, VehicleTypePP>();
			this.m_bufferLock = Singleton<PathManager>.instance.m_bufferLock;
			this.m_pathUnits = Singleton<PathManager>.instance.m_pathUnits;
			this.m_pathFindThread = new Thread(this.PathFindThread);
			this.m_pathFindThread.Name = "Pathfind";
			this.m_pathFindThread.Priority = SimulationManager.SIMULATION_PRIORITY;
			this.m_pathFindThread.Start();
			if (!this.m_pathFindThread.IsAlive)
			{
				CODebugBase<LogChannel>.Error(LogChannel.Core, "Path find thread failed to start!");
			}
		}

		private void OnDestroy()
		{
			while (!Monitor.TryEnter(this.m_queueLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
			{
			}
			try
			{
				this.m_terminated = true;
				Monitor.PulseAll(this.m_queueLock);
			}
			finally
			{
				Monitor.Exit(this.m_queueLock);
			}
		}
	}
}
