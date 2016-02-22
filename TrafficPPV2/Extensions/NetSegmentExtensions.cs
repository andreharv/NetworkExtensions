using System;
using ColossalFramework;
using Transit.Framework.Light;
using UnityEngine;

namespace CSL_Traffic
{
    public static class NetSegmentExtensions
    {
        public static bool GetClosestLanePosition(this NetSegment seg, Vector3 point, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, bool requireConnect, out Vector3 positionA, out int laneIndexA, out float laneOffsetA, out Vector3 positionB, out int laneIndexB, out float laneOffsetB, ExtendedVehicleType vehicleType)
        {
            positionA = point;
            laneIndexA = -1;
            laneOffsetA = 0f;
            positionB = point;
            laneIndexB = -1;
            laneOffsetB = 0f;
            if (seg.m_flags != NetSegment.Flags.None && seg.m_lanes != 0u)
            {
                NetInfo info = seg.Info;
                if (info.m_lanes != null)
                {
                    float num = 1E+09f;
                    float num2 = 1E+09f;
                    uint num3 = seg.m_lanes;
                    int num4 = 0;
                    while (num4 < info.m_lanes.Length && num3 != 0u)
                    {
                        NetInfo.Lane lane = info.m_lanes[num4];
                        if (lane.CheckType(laneTypes, vehicleTypes) && (lane.m_allowConnect || !requireConnect) && RoadManager.CanUseLane(vehicleType, num3))
                        {
                            Vector3 vector;
                            float num5;
                            Singleton<NetManager>.instance.m_lanes.m_buffer[(int)((UIntPtr)num3)].GetClosestPosition(point, out vector, out num5);
                            float num6 = Vector3.SqrMagnitude(point - vector);
                            if (lane.m_finalDirection == NetInfo.Direction.Backward || lane.m_finalDirection == NetInfo.Direction.AvoidForward)
                            {
                                if (num6 < num2)
                                {
                                    num2 = num6;
                                    positionB = vector;
                                    laneIndexB = num4;
                                    laneOffsetB = num5;
                                }
                            }
                            else if (num6 < num)
                            {
                                num = num6;
                                positionA = vector;
                                laneIndexA = num4;
                                laneOffsetA = num5;
                            }
                        }
                        num3 = Singleton<NetManager>.instance.m_lanes.m_buffer[(int)((UIntPtr)num3)].m_nextLane;
                        num4++;
                    }
                    if (num2 < num)
                    {
                        Vector3 vector2 = positionA;
                        int num7 = laneIndexA;
                        float num8 = laneOffsetA;
                        positionA = positionB;
                        laneIndexA = laneIndexB;
                        laneOffsetA = laneOffsetB;
                        positionB = vector2;
                        laneIndexB = num7;
                        laneOffsetB = num8;
                    }
                    if (!info.m_canCrossLanes)
                    {
                        positionB = point;
                        laneIndexB = -1;
                        laneOffsetB = 0f;
                    }
                }
            }
            return laneIndexA != -1;
        }
    }
}