using ColossalFramework;
using ColossalFramework.Math;
using CSL_Traffic.Extensions;
using System;
using System.Threading;
using UnityEngine;
using Transit.Framework.Light;
using Transit.Framework.Unsafe;

namespace CSL_Traffic
{
	/*
	 * The PathManager is needed to use the CustomPathFind class that is where the real magic happens.
	 * There's some work to do here as I have some old code that isn't used anymore.
	 */
	public class CustomPathManager : PathManager
    {
        public static CustomPathFind[] m_pathfinds;

        protected override void Awake()
        {
            PathFind[] originalPathFinds = GetComponents<PathFind>();
            m_pathfinds = new CustomPathFind[originalPathFinds.Length];
            for (int i = 0; i < originalPathFinds.Length; i++)
            {
                Destroy(originalPathFinds[i]);
                m_pathfinds[i] = gameObject.AddComponent<CustomPathFind>();
            }
            typeof(PathManager).GetFieldByName("m_pathfinds").SetValue(this, m_pathfinds);
        }

        // copy values from original to new path manager
        public void SetOriginalValues(PathManager originalPathManager)
        {
            // members of SimulationManagerBase
            this.m_simulationProfiler = originalPathManager.m_simulationProfiler;
            this.m_drawCallData = originalPathManager.m_drawCallData;
            this.m_properties = originalPathManager.m_properties;

            // members of PathManager
            this.m_pathUnitCount = originalPathManager.m_pathUnitCount;
            this.m_renderPathGizmo = originalPathManager.m_renderPathGizmo;
            this.m_pathUnits = originalPathManager.m_pathUnits;
            this.m_bufferLock = originalPathManager.m_bufferLock;
        }

        public static bool FindPathPosition(Vector3 position, ItemClass.Service service, NetInfo.LaneType laneType, VehicleInfo.VehicleType vehicleType, bool allowUnderground, bool requireConnect, float maxDistance, out PathUnit.Position pathPos, ExtendedVehicleType vehicleTypeExtended)
        {
            PathUnit.Position position2;
            float num;
            float num2;
            return FindPathPosition(position, service, laneType, vehicleType, VehicleInfo.VehicleType.None, allowUnderground, requireConnect, maxDistance, out pathPos, out position2, out num, out num2, vehicleTypeExtended);
        }

        public static bool FindPathPosition(Vector3 position, ItemClass.Service service, NetInfo.LaneType laneType, VehicleInfo.VehicleType vehicleType, bool allowUnderground, bool requireConnect, float maxDistance, out PathUnit.Position pathPosA, out PathUnit.Position pathPosB, out float distanceSqrA, out float distanceSqrB, ExtendedVehicleType vehicleTypeExtended)
        {
            return FindPathPosition(position, service, laneType, vehicleType, VehicleInfo.VehicleType.None, allowUnderground, requireConnect, maxDistance, out pathPosA, out pathPosB, out distanceSqrA, out distanceSqrB, vehicleTypeExtended);
        }

        public static bool FindPathPosition(Vector3 position, ItemClass.Service service, NetInfo.LaneType laneType, VehicleInfo.VehicleType vehicleType, VehicleInfo.VehicleType stopType, bool allowUnderground, bool requireConnect, float maxDistance, out PathUnit.Position pathPosA, out PathUnit.Position pathPosB, out float distanceSqrA, out float distanceSqrB, ExtendedVehicleType vehicleTypeExtended)
        {
            Bounds bounds = new Bounds(position, new Vector3(maxDistance * 2f, maxDistance * 2f, maxDistance * 2f));
            int num = Mathf.Max((int)((bounds.min.x - 64f) / 64f + 135f), 0);
            int num2 = Mathf.Max((int)((bounds.min.z - 64f) / 64f + 135f), 0);
            int num3 = Mathf.Min((int)((bounds.max.x + 64f) / 64f + 135f), 269);
            int num4 = Mathf.Min((int)((bounds.max.z + 64f) / 64f + 135f), 269);
            NetManager instance = Singleton<NetManager>.instance;
            pathPosA.m_segment = 0;
            pathPosA.m_lane = 0;
            pathPosA.m_offset = 0;
            distanceSqrA = 1E+10f;
            pathPosB.m_segment = 0;
            pathPosB.m_lane = 0;
            pathPosB.m_offset = 0;
            distanceSqrB = 1E+10f;
            float num5 = maxDistance * maxDistance;
            for (int i = num2; i <= num4; i++)
            {
                for (int j = num; j <= num3; j++)
                {
                    ushort num6 = instance.m_segmentGrid[i * 270 + j];
                    int num7 = 0;
                    while (num6 != 0)
                    {
                        NetInfo info = instance.m_segments.m_buffer[(int)num6].Info;
                        if (info.m_class.m_service == service && (instance.m_segments.m_buffer[(int)num6].m_flags & NetSegment.Flags.Flooded) == NetSegment.Flags.None && (allowUnderground || !info.m_netAI.IsUnderground()))
                        {
                            ushort startNode = instance.m_segments.m_buffer[(int)num6].m_startNode;
                            ushort endNode = instance.m_segments.m_buffer[(int)num6].m_endNode;
                            Vector3 position2 = instance.m_nodes.m_buffer[(int)startNode].m_position;
                            Vector3 position3 = instance.m_nodes.m_buffer[(int)endNode].m_position;
                            float num8 = Mathf.Max(Mathf.Max(bounds.min.x - 64f - position2.x, bounds.min.z - 64f - position2.z), Mathf.Max(position2.x - bounds.max.x - 64f, position2.z - bounds.max.z - 64f));
                            float num9 = Mathf.Max(Mathf.Max(bounds.min.x - 64f - position3.x, bounds.min.z - 64f - position3.z), Mathf.Max(position3.x - bounds.max.x - 64f, position3.z - bounds.max.z - 64f));
                            Vector3 b;
                            int num10;
                            float num11;
                            Vector3 b2;
                            int num12;
                            float num13;
                            if ((num8 < 0f || num9 < 0f) && instance.m_segments.m_buffer[(int)num6].m_bounds.Intersects(bounds) && instance.m_segments.m_buffer[(int)num6].GetClosestLanePosition(position, laneType, vehicleType, stopType, requireConnect, out b, out num10, out num11, out b2, out num12, out num13, vehicleTypeExtended))
                            {
                                float num14 = Vector3.SqrMagnitude(position - b);
                                if (num14 < num5)
                                {
                                    num5 = num14;
                                    pathPosA.m_segment = num6;
                                    pathPosA.m_lane = (byte)num10;
                                    pathPosA.m_offset = (byte)Mathf.Clamp(Mathf.RoundToInt(num11 * 255f), 0, 255);
                                    distanceSqrA = num14;
                                    num14 = Vector3.SqrMagnitude(position - b2);
                                    if (num12 == -1 || num14 >= maxDistance * maxDistance)
                                    {
                                        pathPosB.m_segment = 0;
                                        pathPosB.m_lane = 0;
                                        pathPosB.m_offset = 0;
                                        distanceSqrB = 1E+10f;
                                    }
                                    else
                                    {
                                        pathPosB.m_segment = num6;
                                        pathPosB.m_lane = (byte)num12;
                                        pathPosB.m_offset = (byte)Mathf.Clamp(Mathf.RoundToInt(num13 * 255f), 0, 255);
                                        distanceSqrB = num14;
                                    }
                                }
                            }
                        }
                        num6 = instance.m_segments.m_buffer[(int)num6].m_nextGridSegment;
                        if (++num7 >= 36864)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                            break;
                        }
                    }
                }
            }
            return pathPosA.m_segment != 0;
        }
    }
}
