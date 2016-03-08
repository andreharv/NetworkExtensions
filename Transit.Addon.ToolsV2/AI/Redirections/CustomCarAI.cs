using ColossalFramework;
using ColossalFramework.Math;
using System;
using Transit.Addon.ToolsV2.Data;
using Transit.Framework.Redirection;
using UnityEngine;

namespace CSL_Traffic
{
    public class CustomCarAI : VehicleAI
    {
        [RedirectFrom(typeof(CarAI))]
        protected override void CalculateSegmentPosition(ushort vehicleID, ref Vehicle vehicleData, PathUnit.Position nextPosition, PathUnit.Position position, uint laneID, byte offset, PathUnit.Position prevPos, uint prevLaneID, byte prevOffset, int index, out Vector3 pos, out Vector3 dir, out float maxSpeed)
        {
            NetManager instance = Singleton<NetManager>.instance;
            instance.m_lanes.m_buffer[(int)((UIntPtr)laneID)].CalculatePositionAndDirection((float)offset * 0.003921569f, out pos, out dir);
            Vehicle.Frame lastFrameData = vehicleData.GetLastFrameData();
            Vector3 position2 = lastFrameData.m_position;
            Vector3 b = instance.m_lanes.m_buffer[(int)((UIntPtr)prevLaneID)].CalculatePosition((float)prevOffset * 0.003921569f);
            float num = 0.5f * lastFrameData.m_velocity.sqrMagnitude / this.m_info.m_braking + this.m_info.m_generatedInfo.m_size.z * 0.5f;
            if (Vector3.Distance(position2, b) >= num - 1f)
            {
                Segment3 segment;
                segment.a = pos;
                ushort num2;
                ushort num3;
                if (offset < position.m_offset)
                {
                    segment.b = pos + dir.normalized * this.m_info.m_generatedInfo.m_size.z;
                    num2 = instance.m_segments.m_buffer[(int)position.m_segment].m_startNode;
                    num3 = instance.m_segments.m_buffer[(int)position.m_segment].m_endNode;
                }
                else
                {
                    segment.b = pos - dir.normalized * this.m_info.m_generatedInfo.m_size.z;
                    num2 = instance.m_segments.m_buffer[(int)position.m_segment].m_endNode;
                    num3 = instance.m_segments.m_buffer[(int)position.m_segment].m_startNode;
                }
                ushort num4;
                if (prevOffset == 0)
                {
                    num4 = instance.m_segments.m_buffer[(int)prevPos.m_segment].m_startNode;
                }
                else
                {
                    num4 = instance.m_segments.m_buffer[(int)prevPos.m_segment].m_endNode;
                }
                if (num2 == num4)
                {
                    NetNode.Flags flags = instance.m_nodes.m_buffer[(int)num2].m_flags;
                    NetLane.Flags flags2 = (NetLane.Flags)instance.m_lanes.m_buffer[(int)((UIntPtr)prevLaneID)].m_flags;
                    bool flag = (flags & NetNode.Flags.TrafficLights) != NetNode.Flags.None;
                    bool flag2 = (flags & NetNode.Flags.LevelCrossing) != NetNode.Flags.None;
                    bool flag3 = (flags2 & NetLane.Flags.JoinedJunction) != NetLane.Flags.None;
                    if ((flags & (NetNode.Flags.Junction | NetNode.Flags.OneWayOut | NetNode.Flags.OneWayIn)) == NetNode.Flags.Junction && instance.m_nodes.m_buffer[(int)num2].CountSegments() != 2)
                    {
                        float len = vehicleData.CalculateTotalLength(vehicleID) + 2f;
                        if (!instance.m_lanes.m_buffer[(int)((UIntPtr)laneID)].CheckSpace(len))
                        {
                            bool flag4 = false;
                            if (nextPosition.m_segment != 0 && instance.m_lanes.m_buffer[(int)((UIntPtr)laneID)].m_length < 30f)
                            {
                                NetNode.Flags flags3 = instance.m_nodes.m_buffer[(int)num3].m_flags;
                                if ((flags3 & (NetNode.Flags.Junction | NetNode.Flags.OneWayOut | NetNode.Flags.OneWayIn)) != NetNode.Flags.Junction || instance.m_nodes.m_buffer[(int)num3].CountSegments() == 2)
                                {
                                    uint laneID2 = PathManager.GetLaneID(nextPosition);
                                    if (laneID2 != 0u)
                                    {
                                        flag4 = instance.m_lanes.m_buffer[(int)((UIntPtr)laneID2)].CheckSpace(len);
                                    }
                                }
                            }
                            if (!flag4)
                            {
                                maxSpeed = 0f;
                                return;
                            }
                        }
                    }
                    if (flag && (!flag3 || flag2))
                    {
                        uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
                        uint num5 = (uint)(((int)num4 << 8) / 32768);
                        uint num6 = currentFrameIndex - num5 & 255u;
                        NetInfo info = instance.m_nodes.m_buffer[(int)num2].Info;
                        RoadBaseAI.TrafficLightState vehicleLightState;
                        RoadBaseAI.TrafficLightState pedestrianLightState;
                        bool flag5;
                        bool pedestrians;
                        RoadBaseAI.GetTrafficLightState(num4, ref instance.m_segments.m_buffer[(int)prevPos.m_segment], currentFrameIndex - num5, out vehicleLightState, out pedestrianLightState, out flag5, out pedestrians);
                        if (!flag5 && num6 >= 196u)
                        {
                            flag5 = true;
                            RoadBaseAI.SetTrafficLightState(num4, ref instance.m_segments.m_buffer[(int)prevPos.m_segment], currentFrameIndex - num5, vehicleLightState, pedestrianLightState, flag5, pedestrians);
                        }
                        if ((vehicleData.m_flags & Vehicle.Flags.Emergency2) == Vehicle.Flags.None || info.m_class.m_service != ItemClass.Service.Road)
                        {
                            switch (vehicleLightState)
                            {
                                case RoadBaseAI.TrafficLightState.RedToGreen:
                                    if (num6 < 60u)
                                    {
                                        maxSpeed = 0f;
                                        return;
                                    }
                                    break;
                                case RoadBaseAI.TrafficLightState.Red:
                                    maxSpeed = 0f;
                                    return;
                                case RoadBaseAI.TrafficLightState.GreenToRed:
                                    if (num6 >= 30u)
                                    {
                                        maxSpeed = 0f;
                                        return;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            NetInfo info2 = instance.m_segments.m_buffer[(int)position.m_segment].Info;
            if (info2.m_lanes != null && info2.m_lanes.Length > (int)position.m_lane)
            {
                maxSpeed = this.CalculateTargetSpeed(vehicleID, ref vehicleData, TPPLaneDataManager.GetLaneSpeed(laneID, info2.m_lanes[(int)position.m_lane]), instance.m_lanes.m_buffer[(int)((UIntPtr)laneID)].m_curve);
            }
            else
            {
                maxSpeed = this.CalculateTargetSpeed(vehicleID, ref vehicleData, 1f, 0f);
            }
            if (instance.m_treatWetAsSnow)
            {
                DistrictManager instance2 = Singleton<DistrictManager>.instance;
                byte district = instance2.GetDistrict(pos);
                DistrictPolicies.CityPlanning cityPlanningPolicies = instance2.m_districts.m_buffer[(int)district].m_cityPlanningPolicies;
                if ((cityPlanningPolicies & DistrictPolicies.CityPlanning.StuddedTires) != DistrictPolicies.CityPlanning.None)
                {
                    maxSpeed *= 1f - (float)instance.m_segments.m_buffer[(int)position.m_segment].m_wetness * 0.0005882353f;
                    District[] expr_5C2_cp_0 = instance2.m_districts.m_buffer;
                    byte expr_5C2_cp_1 = district;
                    expr_5C2_cp_0[(int)expr_5C2_cp_1].m_cityPlanningPoliciesEffect = (expr_5C2_cp_0[(int)expr_5C2_cp_1].m_cityPlanningPoliciesEffect | DistrictPolicies.CityPlanning.StuddedTires);
                }
                else
                {
                    maxSpeed *= 1f - (float)instance.m_segments.m_buffer[(int)position.m_segment].m_wetness * 0.00117647066f;
                }
            }
            else
            {
                maxSpeed *= 1f - (float)instance.m_segments.m_buffer[(int)position.m_segment].m_wetness * 0.0005882353f;
            }
            maxSpeed *= 1f + (float)instance.m_segments.m_buffer[(int)position.m_segment].m_condition * 0.0005882353f;
        }
    }
}
