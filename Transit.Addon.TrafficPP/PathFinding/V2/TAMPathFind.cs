using System;
using System.Threading;
using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using UnityEngine;

namespace Transit.Addon.TrafficPP.PathFinding.V2
{
    public partial class TAMPathFind : MonoBehaviour
    {
        private struct BufferItem
        {
            public PathUnit.Position m_position;

            public float m_comparisonValue;

            public float m_methodDistance;

            public uint m_laneID;

            public NetInfo.Direction m_direction;

            public NetInfo.LaneType m_lanesUsed;
        }

        public ThreadProfiler m_pathfindProfiler;

        public volatile int m_queuedPathFindCount;

        private Array32<PathUnit> m_pathUnits;

        private uint m_queueFirst;

        private uint m_queueLast;

        private uint m_calculating;

        private object m_queueLock;

        private object m_bufferLock;

        private Thread m_pathFindThread;

        private bool m_terminated;

        private int m_bufferMinPos;

        private int m_bufferMaxPos;

        private uint[] m_laneLocation;

        private PathUnit.Position[] m_laneTarget;

        private BufferItem[] m_buffer;

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

        public bool IsAvailable
        {
            get
            {
                return this.m_pathFindThread.IsAlive;
            }
        }

        private void Awake()
        {
            this.m_pathfindProfiler = new ThreadProfiler();
            this.m_laneLocation = new uint[262144];
            this.m_laneTarget = new PathUnit.Position[262144];
            this.m_buffer = new BufferItem[65536];
            this.m_bufferMin = new int[1024];
            this.m_bufferMax = new int[1024];
            this.m_queueLock = new object();
            this.m_bufferLock = Singleton<TAMPathManager>.instance.m_bufferLock;
            this.m_pathUnits = Singleton<TAMPathManager>.instance.m_pathUnits;
            this.m_pathFindThread = new Thread(new ThreadStart(this.PathFindThread));
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

        public bool CalculatePath(uint unit, bool skipQueue)
        {
            if (Singleton<TAMPathManager>.instance.AddPathReference(unit))
            {
                while (!Monitor.TryEnter(this.m_queueLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
                {
                }
                try
                {
                    if (skipQueue)
                    {
                        if (this.m_queueLast == 0u)
                        {
                            this.m_queueLast = unit;
                        }
                        else
                        {
                            this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_nextPathUnit = this.m_queueFirst;
                        }
                        this.m_queueFirst = unit;
                    }
                    else
                    {
                        if (this.m_queueLast == 0u)
                        {
                            this.m_queueFirst = unit;
                        }
                        else
                        {
                            this.m_pathUnits.m_buffer[(int)((UIntPtr)this.m_queueLast)].m_nextPathUnit = unit;
                        }
                        this.m_queueLast = unit;
                    }
                    PathUnit[] expr_BD_cp_0 = this.m_pathUnits.m_buffer;
                    UIntPtr expr_BD_cp_1 = (UIntPtr)unit;
                    expr_BD_cp_0[(int)expr_BD_cp_1].m_pathFindFlags = (byte)(expr_BD_cp_0[(int)expr_BD_cp_1].m_pathFindFlags | 1);
                    this.m_queuedPathFindCount++;
                    Monitor.Pulse(this.m_queueLock);
                }
                finally
                {
                    Monitor.Exit(this.m_queueLock);
                }
                return true;
            }
            return false;
        }

        public void WaitForAllPaths()
        {
            while (!Monitor.TryEnter(this.m_queueLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
            {
            }
            try
            {
                while ((this.m_queueFirst != 0u || this.m_calculating != 0u) && !this.m_terminated)
                {
                    Monitor.Wait(this.m_queueLock);
                }
            }
            finally
            {
                Monitor.Exit(this.m_queueLock);
            }
        }

        private float CalculateLaneSpeed(byte startOffset, byte endOffset, ref NetSegment segment, NetInfo.Lane laneInfo)
        {
            NetInfo.Direction direction = ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ? laneInfo.m_finalDirection : NetInfo.InvertDirection(laneInfo.m_finalDirection);
            if ((byte)(direction & NetInfo.Direction.Avoid) == 0)
            {
                return laneInfo.m_speedLimit;
            }
            if (endOffset > startOffset && direction == NetInfo.Direction.AvoidForward)
            {
                return laneInfo.m_speedLimit * 0.1f;
            }
            if (endOffset < startOffset && direction == NetInfo.Direction.AvoidBackward)
            {
                return laneInfo.m_speedLimit * 0.1f;
            }
            return laneInfo.m_speedLimit * 0.2f;
        }

        private void CalculateLaneDirection(PathUnit.Position pathPos, out NetInfo.Direction direction, out NetInfo.LaneType type)
        {
            NetManager instance = Singleton<NetManager>.instance;
            NetInfo info = instance.m_segments.m_buffer[(int)pathPos.m_segment].Info;
            if (info.m_lanes.Length > (int)pathPos.m_lane)
            {
                direction = info.m_lanes[(int)pathPos.m_lane].m_finalDirection;
                type = info.m_lanes[(int)pathPos.m_lane].m_laneType;
                if ((instance.m_segments.m_buffer[(int)pathPos.m_segment].m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None)
                {
                    direction = NetInfo.InvertDirection(direction);
                }
            }
            else
            {
                direction = NetInfo.Direction.None;
                type = NetInfo.LaneType.None;
            }
        }

        private void PathFindThread()
        {
            while (true)
            {
                while (!Monitor.TryEnter(this.m_queueLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
                {
                }
                try
                {
                    while (this.m_queueFirst == 0u && !this.m_terminated)
                    {
                        Monitor.Wait(this.m_queueLock);
                    }
                    if (this.m_terminated)
                    {
                        break;
                    }
                    this.m_calculating = this.m_queueFirst;
                    this.m_queueFirst = this.m_pathUnits.m_buffer[(int)((UIntPtr)this.m_calculating)].m_nextPathUnit;
                    if (this.m_queueFirst == 0u)
                    {
                        this.m_queueLast = 0u;
                        this.m_queuedPathFindCount = 0;
                    }
                    else
                    {
                        this.m_queuedPathFindCount--;
                    }
                    this.m_pathUnits.m_buffer[(int)((UIntPtr)this.m_calculating)].m_nextPathUnit = 0u;
                    this.m_pathUnits.m_buffer[(int)((UIntPtr)this.m_calculating)].m_pathFindFlags = (byte)(((int)this.m_pathUnits.m_buffer[(int)((UIntPtr)this.m_calculating)].m_pathFindFlags & -2) | 2);
                }
                finally
                {
                    Monitor.Exit(this.m_queueLock);
                }
                try
                {
                    this.m_pathfindProfiler.BeginStep();
                    try
                    {
                        this.PathFindImplementation(this.m_calculating, ref this.m_pathUnits.m_buffer[(int)((UIntPtr)this.m_calculating)]);
                    }
                    finally
                    {
                        this.m_pathfindProfiler.EndStep();
                    }
                }
                catch (Exception ex)
                {
                    UIView.ForwardException(ex);
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Path find error: " + ex.Message + "\n" + ex.StackTrace);
                    PathUnit[] expr_1A0_cp_0 = this.m_pathUnits.m_buffer;
                    UIntPtr expr_1A0_cp_1 = (UIntPtr)this.m_calculating;
                    expr_1A0_cp_0[(int)expr_1A0_cp_1].m_pathFindFlags = (byte)(expr_1A0_cp_0[(int)expr_1A0_cp_1].m_pathFindFlags | 8);
                }
                while (!Monitor.TryEnter(this.m_queueLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
                {
                }
                try
                {
                    this.m_pathUnits.m_buffer[(int)((UIntPtr)this.m_calculating)].m_pathFindFlags = (byte)((int)this.m_pathUnits.m_buffer[(int)((UIntPtr)this.m_calculating)].m_pathFindFlags & -3);
                    Singleton<TAMPathManager>.instance.ReleasePath(this.m_calculating);
                    this.m_calculating = 0u;
                    Monitor.Pulse(this.m_queueLock);
                }
                finally
                {
                    Monitor.Exit(this.m_queueLock);
                }
            }
        }
    }
}
