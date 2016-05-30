using ColossalFramework;
using ColossalFramework.Math;
using System;
using UnityEngine;

namespace Transit.Framework.AI
{
    public class CustomMetroTrackBaseAI : PlayerNetAI
    {
        public int m_noiseAccumulation = 10;

        public float m_noiseRadius = 40f;

        public BuildingInfo m_outsideConnection;

        public override Color GetColor(ushort segmentID, ref NetSegment data, InfoManager.InfoMode infoMode)
        {
            if (infoMode == InfoManager.InfoMode.NoisePollution)
            {
                int num = (int)(100 - (data.m_trafficDensity - 100) * (data.m_trafficDensity - 100) / 100);
                int num2 = this.m_noiseAccumulation * num / 100;
                return CommonBuildingAI.GetNoisePollutionColor((float)num2 * 1.25f);
            }
            if (infoMode == InfoManager.InfoMode.Transport)
            {
                return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_neutralColor, Singleton<TransportManager>.instance.m_properties.m_transportColors[2], 0.2f);
            }
            if (infoMode == InfoManager.InfoMode.None)
            {
                Color color = this.m_info.m_color;
                color.a = (float)(255 - data.m_wetness) * 0.003921569f;
                return color;
            }
            if (infoMode != InfoManager.InfoMode.Traffic)
            {
                return base.GetColor(segmentID, ref data, infoMode);
            }
            return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Mathf.Clamp01((float)data.m_trafficDensity * 0.01f));
        }

        public override bool ColorizeProps(InfoManager.InfoMode infoMode)
        {
            return infoMode == InfoManager.InfoMode.Traffic || base.ColorizeProps(infoMode);
        }

        public override Color GetColor(ushort nodeID, ref NetNode data, InfoManager.InfoMode infoMode)
        {
            if (infoMode == InfoManager.InfoMode.NoisePollution)
            {
                int num = 0;
                if (this.m_noiseAccumulation != 0)
                {
                    NetManager instance = Singleton<NetManager>.instance;
                    int num2 = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        ushort segment = data.GetSegment(i);
                        if (segment != 0)
                        {
                            num += (int)instance.m_segments.m_buffer[(int)segment].m_trafficDensity;
                            num2++;
                        }
                    }
                    if (num2 != 0)
                    {
                        num /= num2;
                    }
                }
                int num3 = 100 - (num - 100) * (num - 100) / 100;
                int num4 = this.m_noiseAccumulation * num3 / 100;
                return CommonBuildingAI.GetNoisePollutionColor((float)num4 * 1.25f);
            }
            if (infoMode == InfoManager.InfoMode.Transport)
            {
                return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_neutralColor, Singleton<TransportManager>.instance.m_properties.m_transportColors[2], 0.2f);
            }
            if (infoMode == InfoManager.InfoMode.None)
            {
                int num5 = 0;
                NetManager instance2 = Singleton<NetManager>.instance;
                int num6 = 0;
                for (int j = 0; j < 8; j++)
                {
                    ushort segment2 = data.GetSegment(j);
                    if (segment2 != 0)
                    {
                        num5 += (int)instance2.m_segments.m_buffer[(int)segment2].m_wetness;
                        num6++;
                    }
                }
                if (num6 != 0)
                {
                    num5 /= num6;
                }
                Color color = this.m_info.m_color;
                color.a = (float)(255 - num5) * 0.003921569f;
                return color;
            }
            if (infoMode != InfoManager.InfoMode.Traffic)
            {
                return base.GetColor(nodeID, ref data, infoMode);
            }
            int num7 = 0;
            NetManager instance3 = Singleton<NetManager>.instance;
            int num8 = 0;
            for (int k = 0; k < 8; k++)
            {
                ushort segment3 = data.GetSegment(k);
                if (segment3 != 0)
                {
                    num7 += (int)instance3.m_segments.m_buffer[(int)segment3].m_trafficDensity;
                    num8++;
                }
            }
            if (num8 != 0)
            {
                num7 /= num8;
            }
            return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Mathf.Clamp01((float)num7 * 0.01f));
        }

        public override void GetNodeState(ushort nodeID, ref NetNode nodeData, ushort segmentID, ref NetSegment segmentData, out NetNode.Flags flags, out Color color)
        {
            flags = nodeData.m_flags;
            color = Color.gray.gamma;
            if ((nodeData.m_flags & NetNode.Flags.TrafficLights) != NetNode.Flags.None)
            {
                CustomMetroTrackBaseAI.GetLevelCrossingNodeState(nodeID, ref nodeData, segmentID, ref segmentData, ref flags, ref color);
            }
        }

        public static void GetLevelCrossingNodeState(ushort nodeID, ref NetNode nodeData, ushort segmentID, ref NetSegment segmentData, ref NetNode.Flags flags, ref Color color)
        {
            uint num = Singleton<SimulationManager>.instance.m_referenceFrameIndex - 15u;
            uint num2 = (uint)(((int)nodeID << 8) / 32768);
            uint num3 = num - num2 & 255u;
            float num4 = num3 + Singleton<SimulationManager>.instance.m_referenceTimer;
            RoadBaseAI.TrafficLightState trafficLightState;
            RoadBaseAI.TrafficLightState trafficLightState2;
            RoadBaseAI.GetTrafficLightState(nodeID, ref segmentData, num - num2, out trafficLightState, out trafficLightState2);
            color.a = 0.5f;
            switch (trafficLightState)
            {
                case RoadBaseAI.TrafficLightState.Green:
                    color.g = 1f;
                    color.a = 0.5f;
                    break;
                case RoadBaseAI.TrafficLightState.RedToGreen:
                    if (num3 < 45u)
                    {
                        color.g = 0f;
                    }
                    else if (num3 < 60u)
                    {
                        color.r = 1f;
                    }
                    else
                    {
                        color.g = 1f;
                    }
                    color.a = 0.5f + Mathf.Clamp((90f - num4) / 180f, 0f, 0.5f);
                    break;
                case RoadBaseAI.TrafficLightState.Red:
                    color.g = 0f;
                    color.a = 1f;
                    break;
                case RoadBaseAI.TrafficLightState.GreenToRed:
                    if (num3 < 45u)
                    {
                        color.r = 1f;
                    }
                    else
                    {
                        color.g = 0f;
                    }
                    color.a = 0.5f + Mathf.Clamp(num4 / 180f, 0f, 0.5f);
                    break;
            }
            if (Singleton<SimulationManager>.instance.m_metaData.m_invertTraffic == SimulationMetaData.MetaBool.True)
            {
                color.a = 1f - color.a;
            }
        }

        public override void NodeLoaded(ushort nodeID, ref NetNode data, uint version)
        {
            base.NodeLoaded(nodeID, ref data, version);
            Singleton<NetManager>.instance.AddTileNode(data.m_position, this.m_info.m_class.m_service, this.m_info.m_class.m_subService);
        }

        public override void ManualActivation(ushort segmentID, ref NetSegment data, NetInfo oldInfo)
        {
            if (this.m_noiseAccumulation != 0)
            {
                NetManager instance = Singleton<NetManager>.instance;
                Vector3 position = instance.m_nodes.m_buffer[(int)data.m_startNode].m_position;
                Vector3 position2 = instance.m_nodes.m_buffer[(int)data.m_endNode].m_position;
                Vector3 position3 = (position + position2) * 0.5f;
                float num = Vector3.Distance(position, position2);
                float num2 = (float)this.m_noiseAccumulation * num / this.m_noiseRadius;
                float num3 = Mathf.Max(num, this.m_noiseRadius);
                if (oldInfo != null)
                {
                    int num4;
                    float num5;
                    oldInfo.m_netAI.GetNoiseAccumulation(out num4, out num5);
                    if (num4 != 0)
                    {
                        num2 -= (float)num4 * num / num5;
                        num3 = Mathf.Max(num3, num5);
                    }
                }
                if (num2 != 0f)
                {
                    Singleton<NotificationManager>.instance.AddWaveEvent(position3, (num2 <= 0f) ? NotificationEvent.Type.Happy : NotificationEvent.Type.Sad, ImmaterialResourceManager.Resource.NoisePollution, num2, num3);
                }
            }
        }

        public override void ManualDeactivation(ushort segmentID, ref NetSegment data)
        {
            if (this.m_noiseAccumulation != 0)
            {
                NetManager instance = Singleton<NetManager>.instance;
                Vector3 position = instance.m_nodes.m_buffer[(int)data.m_startNode].m_position;
                Vector3 position2 = instance.m_nodes.m_buffer[(int)data.m_endNode].m_position;
                Vector3 position3 = (position + position2) * 0.5f;
                float num = Vector3.Distance(position, position2);
                float num2 = (float)this.m_noiseAccumulation * num / this.m_noiseRadius;
                float radius = Mathf.Max(num, this.m_noiseRadius);
                Singleton<NotificationManager>.instance.AddWaveEvent(position3, NotificationEvent.Type.Happy, ImmaterialResourceManager.Resource.NoisePollution, -num2, radius);
            }
        }

        public override void GetNoiseAccumulation(out int noiseAccumulation, out float noiseRadius)
        {
            noiseAccumulation = this.m_noiseAccumulation;
            noiseRadius = this.m_noiseRadius;
        }

        public override void TrafficDirectionUpdated(ushort nodeID, ref NetNode data)
        {
            base.TrafficDirectionUpdated(nodeID, ref data);
            this.UpdateOutsideFlags(nodeID, ref data);
        }

        public override void GetNodeBuilding(ushort nodeID, ref NetNode data, out BuildingInfo building, out float heightOffset)
        {
            if ((data.m_flags & NetNode.Flags.Outside) != NetNode.Flags.None)
            {
                building = this.m_outsideConnection;
                heightOffset = -2f;
            }
            else
            {
                base.GetNodeBuilding(nodeID, ref data, out building, out heightOffset);
            }
        }

        public override void SimulationStep(ushort segmentID, ref NetSegment data)
        {
            base.SimulationStep(segmentID, ref data);
            NetManager instance = Singleton<NetManager>.instance;
            Notification.Problem problem = Notification.RemoveProblems(data.m_problems, Notification.Problem.Flood);
            float num = 0f;
            uint num2 = data.m_lanes;
            int num3 = 0;
            while (num3 < this.m_info.m_lanes.Length && num2 != 0u)
            {
                NetInfo.Lane lane = this.m_info.m_lanes[num3];
                if (lane.m_laneType == NetInfo.LaneType.Vehicle)
                {
                    num += instance.m_lanes.m_buffer[(int)((UIntPtr)num2)].m_length;
                }
                num2 = instance.m_lanes.m_buffer[(int)((UIntPtr)num2)].m_nextLane;
                num3++;
            }
            int num4 = Mathf.RoundToInt(num) << 4;
            int num5 = 0;
            if (num4 != 0)
            {
                num5 = (int)((byte)Mathf.Min((int)(data.m_trafficBuffer * 100) / num4, 100));
            }
            data.m_trafficBuffer = 0;
            if (num5 > (int)data.m_trafficDensity)
            {
                data.m_trafficDensity = (byte)Mathf.Min((int)(data.m_trafficDensity + 5), num5);
            }
            else if (num5 < (int)data.m_trafficDensity)
            {
                data.m_trafficDensity = (byte)Mathf.Max((int)(data.m_trafficDensity - 5), num5);
            }
            Vector3 position = instance.m_nodes.m_buffer[(int)data.m_startNode].m_position;
            Vector3 position2 = instance.m_nodes.m_buffer[(int)data.m_endNode].m_position;
            Vector3 vector = (position + position2) * 0.5f;
            bool flag = false;
            if ((this.m_info.m_setVehicleFlags & Vehicle.Flags.Underground) == Vehicle.Flags.None)
            {
                float num6 = Singleton<TerrainManager>.instance.WaterLevel(VectorUtils.XZ(vector));
                if (num6 > vector.y + 1f)
                {
                    flag = true;
                    data.m_flags |= NetSegment.Flags.Flooded;
                    problem = Notification.AddProblems(problem, Notification.Problem.Flood | Notification.Problem.MajorProblem);
                }
                else
                {
                    data.m_flags &= ~NetSegment.Flags.Flooded;
                    if (num6 > vector.y)
                    {
                        flag = true;
                        problem = Notification.AddProblems(problem, Notification.Problem.Flood);
                    }
                }
                int num7 = (int)data.m_wetness;
                if (!instance.m_treatWetAsSnow)
                {
                    if (flag)
                    {
                        num7 = 255;
                    }
                    else
                    {
                        int num8 = -(num7 + 63 >> 5);
                        float num9 = Singleton<WeatherManager>.instance.SampleRainIntensity(vector, false);
                        if (num9 != 0f)
                        {
                            int num10 = Mathf.RoundToInt(Mathf.Min(num9 * 4000f, 1000f));
                            num8 += Singleton<SimulationManager>.instance.m_randomizer.Int32(num10, num10 + 99) / 100;
                        }
                        num7 = Mathf.Clamp(num7 + num8, 0, 255);
                    }
                }
                if (num7 != (int)data.m_wetness)
                {
                    if (Mathf.Abs((int)data.m_wetness - num7) > 10)
                    {
                        data.m_wetness = (byte)num7;
                        InstanceID empty = InstanceID.Empty;
                        empty.NetSegment = segmentID;
                        instance.AddSmoothColor(empty);
                        empty.NetNode = data.m_startNode;
                        instance.AddSmoothColor(empty);
                        empty.NetNode = data.m_endNode;
                        instance.AddSmoothColor(empty);
                    }
                    else
                    {
                        data.m_wetness = (byte)num7;
                        instance.m_wetnessChanged = 256;
                    }
                }
            }
            int num11 = (int)(100 - (data.m_trafficDensity - 100) * (data.m_trafficDensity - 100) / 100);
            int num12 = this.m_noiseAccumulation * num11 / 100;
            if (num12 != 0)
            {
                float num13 = Vector3.Distance(position, position2);
                int num14 = Mathf.FloorToInt(num13 / this.m_noiseRadius);
                for (int i = 0; i < num14; i++)
                {
                    Vector3 position3 = Vector3.Lerp(position, position2, (float)(i + 1) / (float)(num14 + 1));
                    Singleton<ImmaterialResourceManager>.instance.AddResource(ImmaterialResourceManager.Resource.NoisePollution, num12, position3, this.m_noiseRadius);
                }
            }
            data.m_problems = problem;
        }

        public override void SimulationStep(ushort nodeID, ref NetNode data)
        {
            base.SimulationStep(nodeID, ref data);
            if ((data.m_flags & NetNode.Flags.TrafficLights) != NetNode.Flags.None)
            {
                CustomMetroTrackBaseAI.LevelCrossingSimulationStep(nodeID, ref data);
            }
            int num = 0;
            if (this.m_noiseAccumulation != 0)
            {
                NetManager instance = Singleton<NetManager>.instance;
                int num2 = 0;
                for (int i = 0; i < 8; i++)
                {
                    ushort segment = data.GetSegment(i);
                    if (segment != 0)
                    {
                        num += (int)instance.m_segments.m_buffer[(int)segment].m_trafficDensity;
                        num2++;
                    }
                }
                if (num2 != 0)
                {
                    num /= num2;
                }
            }
            int num3 = 100 - (num - 100) * (num - 100) / 100;
            int num4 = this.m_noiseAccumulation * num3 / 100;
            if (num4 != 0)
            {
                Singleton<ImmaterialResourceManager>.instance.AddResource(ImmaterialResourceManager.Resource.NoisePollution, num4, data.m_position, this.m_noiseRadius);
            }
        }

        public static void LevelCrossingSimulationStep(ushort nodeID, ref NetNode data)
        {
            NetManager instance = Singleton<NetManager>.instance;
            uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
            bool flag = false;
            for (int i = 0; i < 8; i++)
            {
                ushort segment = data.GetSegment(i);
                if (segment != 0)
                {
                    NetInfo info = instance.m_segments.m_buffer[(int)segment].Info;
                    if (info.m_class.m_service != ItemClass.Service.Road)
                    {
                        if (info.m_lanes != null)
                        {
                            bool flag2 = instance.m_segments.m_buffer[(int)segment].m_startNode == nodeID;
                            uint num = instance.m_segments.m_buffer[(int)segment].m_lanes;
                            int num2 = 0;
                            while (num2 < info.m_lanes.Length && num != 0u)
                            {
                                if (info.m_lanes[num2].m_laneType == NetInfo.LaneType.Vehicle)
                                {
                                    Vector3 pos = instance.m_lanes.m_buffer[(int)((UIntPtr)num)].CalculatePosition((!flag2) ? 1f : 0f);
                                    if (CustomMetroTrackBaseAI.CheckOverlap(pos))
                                    {
                                        flag = true;
                                    }
                                }
                                num = instance.m_lanes.m_buffer[(int)((UIntPtr)num)].m_nextLane;
                                num2++;
                            }
                        }
                        RoadBaseAI.TrafficLightState trafficLightState;
                        RoadBaseAI.TrafficLightState trafficLightState2;
                        bool flag3;
                        bool flag4;
                        RoadBaseAI.GetTrafficLightState(nodeID, ref instance.m_segments.m_buffer[(int)segment], currentFrameIndex - 256u, out trafficLightState, out trafficLightState2, out flag3, out flag4);
                        if (flag3)
                        {
                            flag = true;
                        }
                    }
                }
            }
            bool flag5 = flag;
            for (int j = 0; j < 8; j++)
            {
                ushort segment2 = data.GetSegment(j);
                if (segment2 != 0)
                {
                    NetInfo info2 = instance.m_segments.m_buffer[(int)segment2].Info;
                    RoadBaseAI.TrafficLightState trafficLightState3;
                    RoadBaseAI.TrafficLightState trafficLightState4;
                    RoadBaseAI.GetTrafficLightState(nodeID, ref instance.m_segments.m_buffer[(int)segment2], currentFrameIndex - 256u, out trafficLightState3, out trafficLightState4);
                    trafficLightState3 &= ~RoadBaseAI.TrafficLightState.RedToGreen;
                    trafficLightState4 &= ~RoadBaseAI.TrafficLightState.RedToGreen;
                    if (info2.m_class.m_service == ItemClass.Service.Road)
                    {
                        if (flag5)
                        {
                            if ((trafficLightState3 & RoadBaseAI.TrafficLightState.Red) == RoadBaseAI.TrafficLightState.Green)
                            {
                                trafficLightState3 = RoadBaseAI.TrafficLightState.GreenToRed;
                            }
                        }
                        else if ((trafficLightState3 & RoadBaseAI.TrafficLightState.Red) != RoadBaseAI.TrafficLightState.Green)
                        {
                            trafficLightState3 = RoadBaseAI.TrafficLightState.RedToGreen;
                        }
                        trafficLightState4 = RoadBaseAI.TrafficLightState.Green;
                    }
                    else if (flag5)
                    {
                        if ((trafficLightState3 & RoadBaseAI.TrafficLightState.Red) != RoadBaseAI.TrafficLightState.Green)
                        {
                            trafficLightState3 = RoadBaseAI.TrafficLightState.RedToGreen;
                        }
                        if ((trafficLightState4 & RoadBaseAI.TrafficLightState.Red) == RoadBaseAI.TrafficLightState.Green)
                        {
                            trafficLightState4 = RoadBaseAI.TrafficLightState.GreenToRed;
                        }
                    }
                    else
                    {
                        if ((trafficLightState3 & RoadBaseAI.TrafficLightState.Red) == RoadBaseAI.TrafficLightState.Green)
                        {
                            trafficLightState3 = RoadBaseAI.TrafficLightState.GreenToRed;
                        }
                        if ((trafficLightState4 & RoadBaseAI.TrafficLightState.Red) != RoadBaseAI.TrafficLightState.Green)
                        {
                            trafficLightState4 = RoadBaseAI.TrafficLightState.RedToGreen;
                        }
                    }
                    RoadBaseAI.SetTrafficLightState(nodeID, ref instance.m_segments.m_buffer[(int)segment2], currentFrameIndex, trafficLightState3, trafficLightState4, false, false);
                }
            }
        }

        public override void UpdateNodeFlags(ushort nodeID, ref NetNode data)
        {
            base.UpdateNodeFlags(nodeID, ref data);
            NetNode.Flags flags = data.m_flags & ~(NetNode.Flags.Transition | NetNode.Flags.LevelCrossing | NetNode.Flags.TrafficLights);
            int num = 0;
            int num2 = 0;
            NetManager instance = Singleton<NetManager>.instance;
            for (int i = 0; i < 8; i++)
            {
                ushort segment = data.GetSegment(i);
                if (segment != 0)
                {
                    NetInfo info = instance.m_segments.m_buffer[(int)segment].Info;
                    if (info != null)
                    {
                        if (info.m_createPavement)
                        {
                            flags |= NetNode.Flags.Transition;
                        }
                        if (info.m_class.m_service == ItemClass.Service.Road)
                        {
                            num++;
                        }
                        else if (info.m_class.m_service == ItemClass.Service.PublicTransport)
                        {
                            num2++;
                        }
                    }
                }
            }
            if (num >= 2 && num2 >= 2)
            {
                flags |= (NetNode.Flags.LevelCrossing | NetNode.Flags.TrafficLights);
            }
            data.m_flags = flags;
        }

        private static bool CheckOverlap(Vector3 pos)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            int num = Mathf.Max((int)((pos.x - 10f) / 32f + 270f), 0);
            int num2 = Mathf.Max((int)((pos.z - 10f) / 32f + 270f), 0);
            int num3 = Mathf.Min((int)((pos.x + 10f) / 32f + 270f), 539);
            int num4 = Mathf.Min((int)((pos.z + 10f) / 32f + 270f), 539);
            bool result = false;
            for (int i = num2; i <= num4; i++)
            {
                for (int j = num; j <= num3; j++)
                {
                    ushort num5 = instance.m_vehicleGrid[i * 540 + j];
                    int num6 = 0;
                    while (num5 != 0)
                    {
                        num5 = CustomMetroTrackBaseAI.CheckOverlap(pos, num5, ref instance.m_vehicles.m_buffer[(int)num5], ref result);
                        if (++num6 > 16384)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                            break;
                        }
                    }
                }
            }
            return result;
        }

        private static ushort CheckOverlap(Vector3 pos, ushort otherID, ref Vehicle otherData, ref bool overlap)
        {
            float num;
            if (otherData.m_segment.DistanceSqr(pos, out num) < 4f)
            {
                overlap = true;
            }
            return otherData.m_nextGridVehicle;
        }

        public override void UpdateNode(ushort nodeID, ref NetNode data)
        {
            base.UpdateNode(nodeID, ref data);
            Notification.Problem problem = Notification.RemoveProblems(data.m_problems, Notification.Problem.RoadNotConnected);
            int num = 0;
            int num2 = 0;
            data.CountLanes(nodeID, 0, NetInfo.LaneType.Vehicle, VehicleInfo.VehicleType.Train, true, ref num, ref num2);
            if ((data.m_flags & NetNode.Flags.Outside) != NetNode.Flags.None && data.m_building != 0)
            {
                BuildingManager instance = Singleton<BuildingManager>.instance;
                if (num != 0)
                {
                    Building[] expr_6B_cp_0 = instance.m_buildings.m_buffer;
                    ushort expr_6B_cp_1 = data.m_building;
                    expr_6B_cp_0[(int)expr_6B_cp_1].m_flags = (expr_6B_cp_0[(int)expr_6B_cp_1].m_flags | Building.Flags.Outgoing);
                }
                else
                {
                    Building[] expr_97_cp_0 = instance.m_buildings.m_buffer;
                    ushort expr_97_cp_1 = data.m_building;
                    expr_97_cp_0[(int)expr_97_cp_1].m_flags = (expr_97_cp_0[(int)expr_97_cp_1].m_flags & ~Building.Flags.Outgoing);
                }
                if (num2 != 0)
                {
                    Building[] expr_C4_cp_0 = instance.m_buildings.m_buffer;
                    ushort expr_C4_cp_1 = data.m_building;
                    expr_C4_cp_0[(int)expr_C4_cp_1].m_flags = (expr_C4_cp_0[(int)expr_C4_cp_1].m_flags | Building.Flags.Incoming);
                }
                else
                {
                    Building[] expr_ED_cp_0 = instance.m_buildings.m_buffer;
                    ushort expr_ED_cp_1 = data.m_building;
                    expr_ED_cp_0[(int)expr_ED_cp_1].m_flags = (expr_ED_cp_0[(int)expr_ED_cp_1].m_flags & ~Building.Flags.Incoming);
                }
            }
            else if ((num != 0 && num2 == 0) || (num == 0 && num2 != 0))
            {
                problem = Notification.AddProblems(problem, Notification.Problem.RoadNotConnected);
            }
            num = 0;
            num2 = 0;
            data.CountLanes(nodeID, 0, NetInfo.LaneType.Vehicle, VehicleInfo.VehicleType.Car, true, ref num, ref num2);
            if ((num != 0 && num2 == 0) || (num == 0 && num2 != 0))
            {
                problem = Notification.AddProblems(problem, Notification.Problem.RoadNotConnected);
            }
            data.m_problems = problem;
        }

        private void UpdateOutsideFlags(ushort nodeID, ref NetNode data)
        {
            if ((data.m_flags & NetNode.Flags.Outside) != NetNode.Flags.None && data.m_building != 0)
            {
                int num = 0;
                int num2 = 0;
                data.CountLanes(nodeID, 0, NetInfo.LaneType.Vehicle, VehicleInfo.VehicleType.Train, true, ref num, ref num2);
                BuildingManager instance = Singleton<BuildingManager>.instance;
                if (num != 0)
                {
                    Building[] expr_51_cp_0 = instance.m_buildings.m_buffer;
                    ushort expr_51_cp_1 = data.m_building;
                    expr_51_cp_0[(int)expr_51_cp_1].m_flags = (expr_51_cp_0[(int)expr_51_cp_1].m_flags | Building.Flags.Outgoing);
                }
                else
                {
                    Building[] expr_7D_cp_0 = instance.m_buildings.m_buffer;
                    ushort expr_7D_cp_1 = data.m_building;
                    expr_7D_cp_0[(int)expr_7D_cp_1].m_flags = (expr_7D_cp_0[(int)expr_7D_cp_1].m_flags & ~Building.Flags.Outgoing);
                }
                if (num2 != 0)
                {
                    Building[] expr_AA_cp_0 = instance.m_buildings.m_buffer;
                    ushort expr_AA_cp_1 = data.m_building;
                    expr_AA_cp_0[(int)expr_AA_cp_1].m_flags = (expr_AA_cp_0[(int)expr_AA_cp_1].m_flags | Building.Flags.Incoming);
                }
                else
                {
                    Building[] expr_D3_cp_0 = instance.m_buildings.m_buffer;
                    ushort expr_D3_cp_1 = data.m_building;
                    expr_D3_cp_0[(int)expr_D3_cp_1].m_flags = (expr_D3_cp_0[(int)expr_D3_cp_1].m_flags & ~Building.Flags.Incoming);
                }
            }
        }

        public override void UpdateLanes(ushort segmentID, ref NetSegment data, bool loading)
        {
            base.UpdateLanes(segmentID, ref data, loading);
            if (!loading)
            {
                NetManager instance = Singleton<NetManager>.instance;
                int num = Mathf.Max((int)((data.m_bounds.min.x - 16f) / 64f + 135f), 0);
                int num2 = Mathf.Max((int)((data.m_bounds.min.z - 16f) / 64f + 135f), 0);
                int num3 = Mathf.Min((int)((data.m_bounds.max.x + 16f) / 64f + 135f), 269);
                int num4 = Mathf.Min((int)((data.m_bounds.max.z + 16f) / 64f + 135f), 269);
                for (int i = num2; i <= num4; i++)
                {
                    for (int j = num; j <= num3; j++)
                    {
                        ushort num5 = instance.m_nodeGrid[i * 270 + j];
                        int num6 = 0;
                        while (num5 != 0)
                        {
                            NetInfo info = instance.m_nodes.m_buffer[(int)num5].Info;
                            Vector3 position = instance.m_nodes.m_buffer[(int)num5].m_position;
                            float num7 = Mathf.Max(Mathf.Max(data.m_bounds.min.x - 16f - position.x, data.m_bounds.min.z - 16f - position.z), Mathf.Max(position.x - data.m_bounds.max.x - 16f, position.z - data.m_bounds.max.z - 16f));
                            if (num7 < 0f)
                            {
                                info.m_netAI.NearbyLanesUpdated(num5, ref instance.m_nodes.m_buffer[(int)num5]);
                            }
                            num5 = instance.m_nodes.m_buffer[(int)num5].m_nextGridNode;
                            if (++num6 >= 32768)
                            {
                                CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public override ToolBase.ToolErrors CanConnectTo(ushort node, ushort segment, ulong[] segmentMask)
        {
            ToolBase.ToolErrors toolErrors = base.CanConnectTo(node, segment, segmentMask);
            NetManager instance = Singleton<NetManager>.instance;
            if (node != 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    ushort segment2 = instance.m_nodes.m_buffer[(int)node].GetSegment(i);
                    if (segment2 != 0)
                    {
                        NetInfo info = instance.m_segments.m_buffer[(int)segment2].Info;
                        if (info.m_class.m_service == ItemClass.Service.Road && info.m_netAI.IsOverground())
                        {
                            toolErrors |= ToolBase.ToolErrors.ObjectCollision;
                            if (segmentMask != null)
                            {
                                segmentMask[segment2 >> 6] |= 1uL << (int)segment2;
                            }
                        }
                    }
                }
            }
            else if (segment != 0)
            {
                NetInfo info2 = instance.m_segments.m_buffer[(int)segment].Info;
                if (info2.m_class.m_service == ItemClass.Service.Road && info2.m_netAI.IsOverground())
                {
                    toolErrors |= ToolBase.ToolErrors.ObjectCollision;
                    if (segmentMask != null)
                    {
                        segmentMask[segment >> 6] |= 1uL << (int)segment;
                    }
                }
            }
            return toolErrors;
        }

        public override Color32 GetGroupVertexColor(NetInfo.Segment segmentInfo, int vertexIndex)
        {
            RenderGroup.MeshData data = segmentInfo.m_combinedLod.m_key.m_mesh.m_data;
            Vector3 vector = data.m_vertices[vertexIndex];
            vector.x = vector.x * 0.5f / this.m_info.m_halfWidth + 0.5f;
            vector.z = vector.z / this.m_info.m_segmentLength + 0.5f;
            Color32 result;
            if (data.m_colors != null && data.m_colors.Length > vertexIndex)
            {
                result = data.m_colors[vertexIndex];
            }
            else
            {
                result = new Color32(255, 255, 255, 255);
            }
            Vector3 vector2;
            if (data.m_normals != null && data.m_normals.Length > vertexIndex)
            {
                vector2 = data.m_normals[vertexIndex];
            }
            else
            {
                vector2 = Vector3.up;
            }
            result.b = (byte)Mathf.RoundToInt(vector.z * 255f);
            if (segmentInfo.m_requireSurfaceMaps)
            {
                if (vector.y > -0.31f && vector2.y < 0.8f && vector.x > 0.01f && vector.x < 0.99f)
                {
                    result.g = 255;
                }
                else
                {
                    result.g = 0;
                }
                result.a = 255;
            }
            else
            {
                if (vector2.y > -0.5f)
                {
                    result.g = 255;
                }
                else
                {
                    result.g = 0;
                }
                result.a = 128;
            }
            return result;
        }

        public override Color32 GetGroupVertexColor(NetInfo.Node nodeInfo, int vertexIndex, float vOffset)
        {
            RenderGroup.MeshData data = nodeInfo.m_combinedLod.m_key.m_mesh.m_data;
            Vector3 vector = data.m_vertices[vertexIndex];
            vector.x = vector.x * 0.5f / this.m_info.m_halfWidth + 0.5f;
            Color32 result;
            if (data.m_colors != null && data.m_colors.Length > vertexIndex)
            {
                result = data.m_colors[vertexIndex];
            }
            else
            {
                result = new Color32(255, 255, 255, 255);
            }
            Vector3 vector2;
            if (data.m_normals != null && data.m_normals.Length > vertexIndex)
            {
                vector2 = data.m_normals[vertexIndex];
            }
            else
            {
                vector2 = Vector3.up;
            }
            result.b = (byte)Mathf.Clamp(Mathf.RoundToInt(vOffset * 255f), 0, 255);
            if (nodeInfo.m_requireSurfaceMaps)
            {
                if (vector.y > -0.31f && vector2.y < 0.8f && vector.x > 0.01f && vector.x < 0.99f)
                {
                    result.g = 255;
                }
                else
                {
                    result.g = 0;
                }
                result.a = 255;
            }
            else
            {
                if (vector2.y > -0.5f)
                {
                    result.g = 255;
                }
                else
                {
                    result.g = 0;
                }
                result.a = 128;
            }
            return result;
        }

        public override void GetRayCastHeights(ushort segmentID, ref NetSegment data, out float leftMin, out float rightMin, out float max)
        {
            leftMin = this.m_info.m_minHeight;
            rightMin = this.m_info.m_minHeight;
            max = 0f;
        }
    }
}
