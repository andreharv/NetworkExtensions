//using ColossalFramework;
//using ColossalFramework.IO;
//using ColossalFramework.Math;
//using System;
//using System.Threading;
//using UnityEngine;

//public class PathManager : SimulationManagerBase<PathManager, PathProperties>, ISimulationManager
//{
//    public class Data : IDataContainer
//    {
//        public void Serialize(DataSerializer s)
//        {
//            Singleton<LoadingManager>.instance.m_loadingProfilerSimulation.BeginSerialize(s, "PathManager");
//            PathManager instance = Singleton<PathManager>.instance;
//            instance.WaitForAllPaths();
//            PathUnit[] buffer = instance.m_pathUnits.m_buffer;
//            int num = buffer.Length;
//            EncodedArray.Byte @byte = EncodedArray.Byte.BeginWrite(s);
//            for (int i = 1; i < num; i++)
//            {
//                @byte.Write(buffer[i].m_simulationFlags);
//            }
//            @byte.EndWrite();
//            for (int j = 1; j < num; j++)
//            {
//                if (buffer[j].m_simulationFlags != 0)
//                {
//                    s.WriteUInt8((uint)buffer[j].m_pathFindFlags);
//                    s.WriteUInt8((uint)buffer[j].m_laneTypes);
//                    s.WriteUInt8((uint)buffer[j].m_vehicleTypes);
//                    s.WriteUInt8((uint)buffer[j].m_positionCount);
//                    s.WriteUInt24(buffer[j].m_nextPathUnit);
//                    s.WriteUInt32(buffer[j].m_buildIndex);
//                    s.WriteFloat(buffer[j].m_length);
//                    int positionCount = (int)buffer[j].m_positionCount;
//                    for (int k = 0; k < positionCount; k++)
//                    {
//                        PathUnit.Position position = buffer[j].GetPosition(k);
//                        s.WriteUInt16((uint)position.m_segment);
//                        s.WriteUInt8((uint)position.m_offset);
//                        s.WriteUInt8((uint)position.m_lane);
//                    }
//                }
//            }
//            Singleton<LoadingManager>.instance.m_loadingProfilerSimulation.EndSerialize(s, "PathManager");
//        }

//        public void Deserialize(DataSerializer s)
//        {
//            Singleton<LoadingManager>.instance.m_loadingProfilerSimulation.BeginDeserialize(s, "PathManager");
//            PathManager instance = Singleton<PathManager>.instance;
//            instance.WaitForAllPaths();
//            PathUnit[] buffer = instance.m_pathUnits.m_buffer;
//            int num = buffer.Length;
//            instance.m_pathUnits.ClearUnused();
//            if (s.version < 123u)
//            {
//                num = 131072;
//            }
//            if (s.version >= 46u)
//            {
//                EncodedArray.Byte @byte = EncodedArray.Byte.BeginRead(s);
//                for (int i = 1; i < num; i++)
//                {
//                    buffer[i].m_simulationFlags = @byte.Read();
//                }
//                @byte.EndRead();
//            }
//            else
//            {
//                for (int j = 1; j < num; j++)
//                {
//                    buffer[j].m_simulationFlags = 0;
//                }
//            }
//            for (int k = 1; k < num; k++)
//            {
//                buffer[k].m_referenceCount = 0;
//                if (buffer[k].m_simulationFlags != 0)
//                {
//                    buffer[k].m_pathFindFlags = (byte)s.ReadUInt8();
//                    if (s.version >= 49u)
//                    {
//                        buffer[k].m_laneTypes = (byte)s.ReadUInt8();
//                    }
//                    else
//                    {
//                        buffer[k].m_laneTypes = 3;
//                    }
//                    buffer[k].m_vehicleTypes = (byte)s.ReadUInt8();
//                    buffer[k].m_positionCount = (byte)s.ReadUInt8();
//                    buffer[k].m_nextPathUnit = s.ReadUInt24();
//                    buffer[k].m_buildIndex = s.ReadUInt32();
//                    buffer[k].m_length = s.ReadFloat();
//                    int positionCount = (int)buffer[k].m_positionCount;
//                    for (int l = 0; l < positionCount; l++)
//                    {
//                        PathUnit.Position position;
//                        position.m_segment = (ushort)s.ReadUInt16();
//                        position.m_offset = (byte)s.ReadUInt8();
//                        position.m_lane = (byte)s.ReadUInt8();
//                        buffer[k].SetPosition(l, position);
//                    }
//                }
//                else
//                {
//                    buffer[k].m_pathFindFlags = 0;
//                    buffer[k].m_laneTypes = 0;
//                    buffer[k].m_vehicleTypes = 0;
//                    buffer[k].m_positionCount = 0;
//                    buffer[k].m_nextPathUnit = 0u;
//                    buffer[k].m_buildIndex = 0u;
//                    buffer[k].m_length = 0f;
//                    instance.m_pathUnits.ReleaseItem((uint)k);
//                }
//            }
//            if (s.version < 123u)
//            {
//                int num2 = buffer.Length;
//                for (int m = num; m < num2; m++)
//                {
//                    buffer[m].m_referenceCount = 0;
//                    buffer[m].m_simulationFlags = 0;
//                    buffer[m].m_pathFindFlags = 0;
//                    buffer[m].m_laneTypes = 0;
//                    buffer[m].m_vehicleTypes = 0;
//                    buffer[m].m_positionCount = 0;
//                    buffer[m].m_nextPathUnit = 0u;
//                    buffer[m].m_buildIndex = 0u;
//                    buffer[m].m_length = 0f;
//                    instance.m_pathUnits.ReleaseItem((uint)m);
//                }
//            }
//            Singleton<LoadingManager>.instance.m_loadingProfilerSimulation.EndDeserialize(s, "PathManager");
//        }

//        public void AfterDeserialize(DataSerializer s)
//        {
//            Singleton<LoadingManager>.instance.m_loadingProfilerSimulation.BeginAfterDeserialize(s, "PathManager");
//            Singleton<LoadingManager>.instance.WaitUntilEssentialScenesLoaded();
//            PathManager instance = Singleton<PathManager>.instance;
//            PathUnit[] buffer = instance.m_pathUnits.m_buffer;
//            int num = buffer.Length;
//            for (int i = 1; i < num; i++)
//            {
//                if (buffer[i].m_simulationFlags != 0 && buffer[i].m_nextPathUnit != 0u)
//                {
//                    PathUnit[] expr_71_cp_0 = buffer;
//                    UIntPtr expr_71_cp_1 = (UIntPtr)buffer[i].m_nextPathUnit;
//                    expr_71_cp_0[(int)expr_71_cp_1].m_referenceCount = expr_71_cp_0[(int)expr_71_cp_1].m_referenceCount + 1;
//                }
//            }
//            instance.m_pathUnitCount = (int)(instance.m_pathUnits.ItemCount() - 1u);
//            Singleton<LoadingManager>.instance.m_loadingProfilerSimulation.EndAfterDeserialize(s, "PathManager");
//        }
//    }

//    public const int MAX_PATHUNIT_COUNT = 262144;

//    public int m_pathUnitCount;

//    public int m_renderPathGizmo;

//    [NonSerialized]
//    public Array32<PathUnit> m_pathUnits;

//    [NonSerialized]
//    public object m_bufferLock;

//    private PathFind[] m_pathfinds;

//    private bool m_terminated;

//    protected override void Awake()
//    {
//        base.Awake();
//        this.m_pathUnits = new Array32<PathUnit>(262144u);
//        this.m_bufferLock = new object();
//        uint num;
//        this.m_pathUnits.CreateItem(out num);
//        int num2 = Mathf.Clamp(SystemInfo.processorCount / 2, 1, 4);
//        this.m_pathfinds = new PathFind[num2];
//        for (int i = 0; i < num2; i++)
//        {
//            this.m_pathfinds[i] = base.gameObject.AddComponent<PathFind>();
//        }
//    }

//    private void OnDestroy()
//    {
//        this.m_terminated = true;
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.red;
//        int num = 0;
//        for (uint num2 = 0u; num2 < this.m_pathUnits.m_size; num2 += 1u)
//        {
//            if ((this.m_pathUnits.m_buffer[(int)((UIntPtr)num2)].m_pathFindFlags & 4) != 0)
//            {
//                int positionCount = (int)this.m_pathUnits.m_buffer[(int)((UIntPtr)num2)].m_positionCount;
//                if (positionCount >= 1 && num++ == this.m_renderPathGizmo)
//                {
//                    PathUnit.Position pathPos;
//                    if (this.m_pathUnits.m_buffer[(int)((UIntPtr)num2)].GetPosition(0, out pathPos))
//                    {
//                        Vector3 vector = PathManager.CalculatePosition(pathPos);
//                        Gizmos.DrawSphere(vector, 8f);
//                        for (int i = 1; i < positionCount; i++)
//                        {
//                            if (this.m_pathUnits.m_buffer[(int)((UIntPtr)num2)].GetPosition(i, out pathPos))
//                            {
//                                Vector3 vector2 = PathManager.CalculatePosition(pathPos);
//                                Gizmos.DrawLine(vector, vector2);
//                                vector = vector2;
//                            }
//                        }
//                        if (positionCount >= 2)
//                        {
//                            Gizmos.DrawSphere(vector, 8f);
//                        }
//                    }
//                    break;
//                }
//            }
//        }
//    }

//    public static bool FindPathPosition(Vector3 position, ItemClass.Service service, NetInfo.LaneType laneType, VehicleInfo.VehicleType vehicleType, bool allowUnderground, bool requireConnect, float maxDistance, out PathUnit.Position pathPos)
//    {
//        PathUnit.Position position2;
//        float num;
//        float num2;
//        return PathManager.FindPathPosition(position, service, laneType, vehicleType, allowUnderground, requireConnect, maxDistance, out pathPos, out position2, out num, out num2);
//    }

//    public static bool FindPathPosition(Vector3 position, ItemClass.Service service, NetInfo.LaneType laneType, VehicleInfo.VehicleType vehicleType, bool allowUnderground, bool requireConnect, float maxDistance, out PathUnit.Position pathPosA, out PathUnit.Position pathPosB, out float distanceSqrA, out float distanceSqrB)
//    {
//        Bounds bounds = new Bounds(position, new Vector3(maxDistance * 2f, maxDistance * 2f, maxDistance * 2f));
//        int num = Mathf.Max((int)((bounds.min.x - 64f) / 64f + 135f), 0);
//        int num2 = Mathf.Max((int)((bounds.min.z - 64f) / 64f + 135f), 0);
//        int num3 = Mathf.Min((int)((bounds.max.x + 64f) / 64f + 135f), 269);
//        int num4 = Mathf.Min((int)((bounds.max.z + 64f) / 64f + 135f), 269);
//        NetManager instance = Singleton<NetManager>.instance;
//        pathPosA.m_segment = 0;
//        pathPosA.m_lane = 0;
//        pathPosA.m_offset = 0;
//        distanceSqrA = 1E+10f;
//        pathPosB.m_segment = 0;
//        pathPosB.m_lane = 0;
//        pathPosB.m_offset = 0;
//        distanceSqrB = 1E+10f;
//        float num5 = maxDistance * maxDistance;
//        for (int i = num2; i <= num4; i++)
//        {
//            for (int j = num; j <= num3; j++)
//            {
//                ushort num6 = instance.m_segmentGrid[i * 270 + j];
//                int num7 = 0;
//                while (num6 != 0)
//                {
//                    NetInfo info = instance.m_segments.m_buffer[(int)num6].Info;
//                    if (info.m_class.m_service == service && (instance.m_segments.m_buffer[(int)num6].m_flags & NetSegment.Flags.Flooded) == NetSegment.Flags.None && (allowUnderground || !info.m_netAI.IsUnderground()))
//                    {
//                        ushort startNode = instance.m_segments.m_buffer[(int)num6].m_startNode;
//                        ushort endNode = instance.m_segments.m_buffer[(int)num6].m_endNode;
//                        Vector3 position2 = instance.m_nodes.m_buffer[(int)startNode].m_position;
//                        Vector3 position3 = instance.m_nodes.m_buffer[(int)endNode].m_position;
//                        float num8 = Mathf.Max(Mathf.Max(bounds.min.x - 64f - position2.x, bounds.min.z - 64f - position2.z), Mathf.Max(position2.x - bounds.max.x - 64f, position2.z - bounds.max.z - 64f));
//                        float num9 = Mathf.Max(Mathf.Max(bounds.min.x - 64f - position3.x, bounds.min.z - 64f - position3.z), Mathf.Max(position3.x - bounds.max.x - 64f, position3.z - bounds.max.z - 64f));
//                        Vector3 b;
//                        int num10;
//                        float num11;
//                        Vector3 b2;
//                        int num12;
//                        float num13;
//                        if ((num8 < 0f || num9 < 0f) && instance.m_segments.m_buffer[(int)num6].m_bounds.Intersects(bounds) && instance.m_segments.m_buffer[(int)num6].GetClosestLanePosition(position, laneType, vehicleType, requireConnect, out b, out num10, out num11, out b2, out num12, out num13))
//                        {
//                            float num14 = Vector3.SqrMagnitude(position - b);
//                            if (num14 < num5)
//                            {
//                                num5 = num14;
//                                pathPosA.m_segment = num6;
//                                pathPosA.m_lane = (byte)num10;
//                                pathPosA.m_offset = (byte)Mathf.Clamp(Mathf.RoundToInt(num11 * 255f), 0, 255);
//                                distanceSqrA = num14;
//                                num14 = Vector3.SqrMagnitude(position - b2);
//                                if (num12 == -1 || num14 >= maxDistance * maxDistance)
//                                {
//                                    pathPosB.m_segment = 0;
//                                    pathPosB.m_lane = 0;
//                                    pathPosB.m_offset = 0;
//                                    distanceSqrB = 1E+10f;
//                                }
//                                else
//                                {
//                                    pathPosB.m_segment = num6;
//                                    pathPosB.m_lane = (byte)num12;
//                                    pathPosB.m_offset = (byte)Mathf.Clamp(Mathf.RoundToInt(num13 * 255f), 0, 255);
//                                    distanceSqrB = num14;
//                                }
//                            }
//                        }
//                    }
//                    num6 = instance.m_segments.m_buffer[(int)num6].m_nextGridSegment;
//                    if (++num7 >= 32768)
//                    {
//                        CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
//                        break;
//                    }
//                }
//            }
//        }
//        return pathPosA.m_segment != 0;
//    }

//    public static Vector3 CalculatePosition(PathUnit.Position pathPos)
//    {
//        NetManager instance = Singleton<NetManager>.instance;
//        uint num = instance.m_segments.m_buffer[(int)pathPos.m_segment].m_lanes;
//        int num2 = 0;
//        while (num2 < (int)pathPos.m_lane && num != 0u)
//        {
//            num = instance.m_lanes.m_buffer[(int)((UIntPtr)num)].m_nextLane;
//            num2++;
//        }
//        if (num != 0u)
//        {
//            return instance.m_lanes.m_buffer[(int)((UIntPtr)num)].CalculatePosition((float)pathPos.m_offset * 0.003921569f);
//        }
//        return Vector3.zero;
//    }

//    public static uint GetLaneID(PathUnit.Position pathPos)
//    {
//        NetManager instance = Singleton<NetManager>.instance;
//        uint num = instance.m_segments.m_buffer[(int)pathPos.m_segment].m_lanes;
//        int num2 = 0;
//        while (num2 < (int)pathPos.m_lane && num != 0u)
//        {
//            num = instance.m_lanes.m_buffer[(int)((UIntPtr)num)].m_nextLane;
//            num2++;
//        }
//        return num;
//    }

//    public bool CreatePath(out uint unit, ref Randomizer randomizer, uint buildIndex, PathUnit.Position startPos, PathUnit.Position endPos, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, float maxLength)
//    {
//        PathUnit.Position position = default(PathUnit.Position);
//        return this.CreatePath(out unit, ref randomizer, buildIndex, startPos, position, endPos, position, position, laneTypes, vehicleTypes, maxLength, false, false, false, false);
//    }

//    public bool CreatePath(out uint unit, ref Randomizer randomizer, uint buildIndex, PathUnit.Position startPosA, PathUnit.Position startPosB, PathUnit.Position endPosA, PathUnit.Position endPosB, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, float maxLength)
//    {
//        return this.CreatePath(out unit, ref randomizer, buildIndex, startPosA, startPosB, endPosA, endPosB, default(PathUnit.Position), laneTypes, vehicleTypes, maxLength, false, false, false, false);
//    }

//    public bool CreatePath(out uint unit, ref Randomizer randomizer, uint buildIndex, PathUnit.Position startPosA, PathUnit.Position startPosB, PathUnit.Position endPosA, PathUnit.Position endPosB, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, float maxLength, bool isHeavyVehicle, bool ignoreBlocked, bool stablePath, bool skipQueue)
//    {
//        return this.CreatePath(out unit, ref randomizer, buildIndex, startPosA, startPosB, endPosA, endPosB, default(PathUnit.Position), laneTypes, vehicleTypes, maxLength, isHeavyVehicle, ignoreBlocked, stablePath, skipQueue);
//    }

//    public bool CreatePath(out uint unit, ref Randomizer randomizer, uint buildIndex, PathUnit.Position startPosA, PathUnit.Position startPosB, PathUnit.Position endPosA, PathUnit.Position endPosB, PathUnit.Position vehiclePosition, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, float maxLength, bool isHeavyVehicle, bool ignoreBlocked, bool stablePath, bool skipQueue)
//    {
//        while (!Monitor.TryEnter(this.m_bufferLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
//        {
//        }
//        uint num;
//        try
//        {
//            if (!this.m_pathUnits.CreateItem(out num, ref randomizer))
//            {
//                unit = 0u;
//                bool result = false;
//                return result;
//            }
//            this.m_pathUnitCount = (int)(this.m_pathUnits.ItemCount() - 1u);
//        }
//        finally
//        {
//            Monitor.Exit(this.m_bufferLock);
//        }
//        unit = num;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_simulationFlags = 1;
//        if (isHeavyVehicle)
//        {
//            PathUnit[] expr_92_cp_0 = this.m_pathUnits.m_buffer;
//            UIntPtr expr_92_cp_1 = (UIntPtr)unit;
//            expr_92_cp_0[(int)expr_92_cp_1].m_simulationFlags = (expr_92_cp_0[(int)expr_92_cp_1].m_simulationFlags | 16);
//        }
//        if (ignoreBlocked)
//        {
//            PathUnit[] expr_BB_cp_0 = this.m_pathUnits.m_buffer;
//            UIntPtr expr_BB_cp_1 = (UIntPtr)unit;
//            expr_BB_cp_0[(int)expr_BB_cp_1].m_simulationFlags = (expr_BB_cp_0[(int)expr_BB_cp_1].m_simulationFlags | 32);
//        }
//        if (stablePath)
//        {
//            PathUnit[] expr_E4_cp_0 = this.m_pathUnits.m_buffer;
//            UIntPtr expr_E4_cp_1 = (UIntPtr)unit;
//            expr_E4_cp_0[(int)expr_E4_cp_1].m_simulationFlags = (expr_E4_cp_0[(int)expr_E4_cp_1].m_simulationFlags | 64);
//        }
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_pathFindFlags = 0;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_buildIndex = buildIndex;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_position00 = startPosA;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_position01 = endPosA;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_position02 = startPosB;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_position03 = endPosB;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_position11 = vehiclePosition;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_nextPathUnit = 0u;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_laneTypes = (byte)laneTypes;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_vehicleTypes = (byte)vehicleTypes;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_length = maxLength;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_positionCount = 20;
//        this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_referenceCount = 1;
//        int num2 = 10000000;
//        PathFind pathFind = null;
//        for (int i = 0; i < this.m_pathfinds.Length; i++)
//        {
//            PathFind pathFind2 = this.m_pathfinds[i];
//            if (pathFind2.IsAvailable && pathFind2.m_queuedPathFindCount < num2)
//            {
//                num2 = pathFind2.m_queuedPathFindCount;
//                pathFind = pathFind2;
//            }
//        }
//        if (pathFind != null && pathFind.CalculatePath(unit, skipQueue))
//        {
//            return true;
//        }
//        this.ReleasePath(unit);
//        return false;
//    }

//    public void WaitForAllPaths()
//    {
//        for (int i = 0; i < this.m_pathfinds.Length; i++)
//        {
//            this.m_pathfinds[i].WaitForAllPaths();
//        }
//    }

//    public bool AddPathReference(uint unit)
//    {
//        if (unit == 0u)
//        {
//            return false;
//        }
//        if (this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_simulationFlags == 0)
//        {
//            return false;
//        }
//        while (!Monitor.TryEnter(this.m_bufferLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
//        {
//        }
//        bool result;
//        try
//        {
//            if (this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_referenceCount < 255)
//            {
//                PathUnit[] expr_73_cp_0 = this.m_pathUnits.m_buffer;
//                UIntPtr expr_73_cp_1 = (UIntPtr)unit;
//                expr_73_cp_0[(int)expr_73_cp_1].m_referenceCount = expr_73_cp_0[(int)expr_73_cp_1].m_referenceCount + 1;
//                result = true;
//            }
//            else
//            {
//                result = false;
//            }
//        }
//        finally
//        {
//            Monitor.Exit(this.m_bufferLock);
//        }
//        return result;
//    }

//    public void ReleaseFirstUnit(ref uint unit)
//    {
//        if (unit == 0u)
//        {
//            return;
//        }
//        if (this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_simulationFlags == 0)
//        {
//            unit = 0u;
//            return;
//        }
//        while (!Monitor.TryEnter(this.m_bufferLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
//        {
//        }
//        try
//        {
//            uint nextPathUnit = this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_nextPathUnit;
//            if (this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_referenceCount <= 1)
//            {
//                this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_simulationFlags = 0;
//                this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_pathFindFlags = 0;
//                this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_nextPathUnit = 0u;
//                this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_referenceCount = 0;
//                this.m_pathUnits.ReleaseItem(unit);
//                unit = nextPathUnit;
//            }
//            else
//            {
//                PathUnit[] expr_106_cp_0 = this.m_pathUnits.m_buffer;
//                UIntPtr expr_106_cp_1 = (UIntPtr)unit;
//                expr_106_cp_0[(int)expr_106_cp_1].m_referenceCount = expr_106_cp_0[(int)expr_106_cp_1].m_referenceCount - 1;
//                if (this.m_pathUnits.m_buffer[(int)((UIntPtr)nextPathUnit)].m_referenceCount < 255)
//                {
//                    PathUnit[] expr_147_cp_0 = this.m_pathUnits.m_buffer;
//                    UIntPtr expr_147_cp_1 = (UIntPtr)nextPathUnit;
//                    expr_147_cp_0[(int)expr_147_cp_1].m_referenceCount = expr_147_cp_0[(int)expr_147_cp_1].m_referenceCount + 1;
//                    unit = nextPathUnit;
//                }
//                else
//                {
//                    unit = 0u;
//                }
//            }
//            this.m_pathUnitCount = (int)(this.m_pathUnits.ItemCount() - 1u);
//        }
//        finally
//        {
//            Monitor.Exit(this.m_bufferLock);
//        }
//    }

//    public void ReleasePath(uint unit)
//    {
//        if (this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_simulationFlags == 0)
//        {
//            return;
//        }
//        while (!Monitor.TryEnter(this.m_bufferLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
//        {
//        }
//        try
//        {
//            int num = 0;
//            while (unit != 0u)
//            {
//                if (this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_referenceCount > 1)
//                {
//                    PathUnit[] expr_F9_cp_0 = this.m_pathUnits.m_buffer;
//                    UIntPtr expr_F9_cp_1 = (UIntPtr)unit;
//                    expr_F9_cp_0[(int)expr_F9_cp_1].m_referenceCount = expr_F9_cp_0[(int)expr_F9_cp_1].m_referenceCount - 1;
//                    break;
//                }
//                uint nextPathUnit = this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_nextPathUnit;
//                this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_simulationFlags = 0;
//                this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_pathFindFlags = 0;
//                this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_nextPathUnit = 0u;
//                this.m_pathUnits.m_buffer[(int)((UIntPtr)unit)].m_referenceCount = 0;
//                this.m_pathUnits.ReleaseItem(unit);
//                unit = nextPathUnit;
//                if (++num >= 262144)
//                {
//                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
//                    break;
//                }
//            }
//            this.m_pathUnitCount = (int)(this.m_pathUnits.ItemCount() - 1u);
//        }
//        finally
//        {
//            Monitor.Exit(this.m_bufferLock);
//        }
//    }

//    protected override void SimulationStepImpl(int subStep)
//    {
//        int num = 0;
//        for (int i = 0; i < this.m_pathfinds.Length; i++)
//        {
//            num = Mathf.Max(num, this.m_pathfinds[i].m_queuedPathFindCount);
//        }
//        if (num >= 100 && !this.m_terminated)
//        {
//            Thread.Sleep((num - 100) / 100 + 1);
//        }
//    }

//    public override void GetData(FastList<IDataContainer> data)
//    {
//        base.GetData(data);
//        data.Add(new PathManager.Data());
//    }

//    virtual string GetName()
//    {
//        return base.GetName();
//    }

//    virtual ThreadProfiler GetSimulationProfiler()
//    {
//        return base.GetSimulationProfiler();
//    }

//    virtual void SimulationStep(int subStep)
//    {
//        base.SimulationStep(subStep);
//    }
//}
