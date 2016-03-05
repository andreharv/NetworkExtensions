using ColossalFramework;
using System;
using System.Runtime.CompilerServices;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.Network;
using Transit.Framework.Redirection;
using UnityEngine;

namespace CSL_Traffic
{
    public class CustomTransportLineAI : TransportLineAI
    {
        [RedirectFrom(typeof(TransportLineAI))]
        public override void UpdateLaneConnection(ushort nodeID, ref NetNode data)
        {
            if ((data.m_flags & NetNode.Flags.Temporary) == NetNode.Flags.None)
            {
                uint num = 0u;
                byte offset = 0;
                float num2 = 1E+10f;
                PathUnit.Position pathPos;
                PathUnit.Position position;
                float num3;
                float num4;
                if ((data.m_flags & NetNode.Flags.ForbidLaneConnection) == NetNode.Flags.None && PathManager.FindPathPosition(data.m_position, this.m_netService, NetInfo.LaneType.Pedestrian, VehicleInfo.VehicleType.None, this.m_vehicleType, true, false, 32f, out pathPos, out position, out num3, out num4) && num3 < num2)
                {
                    NetManager instance = Singleton<NetManager>.instance;
                    int num5;
                    uint num6;
                    if (instance.m_segments.m_buffer[(int)pathPos.m_segment].GetClosestLane((int)pathPos.m_lane, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, this.m_vehicleType, out num5, out num6))
                    {
                        num = PathManager.GetLaneID(pathPos);
                        offset = pathPos.m_offset;
                    }
                }
                if (num != data.m_lane)
                {
                    if (data.m_lane != 0u)
                    {
                        this.RemoveLaneConnection(nodeID, ref data);
                    }
                    if (num != 0u)
                    {
                        this.AddLaneConnection(nodeID, ref data, num, offset);
                    }
                }
            }
        }

        [RedirectFrom(typeof(TransportLineAI))]
        public static new bool StartPathFind(ushort segmentID, ref NetSegment data, ItemClass.Service netService, VehicleInfo.VehicleType vehicleType, bool skipQueue)
		{
            if (data.m_path != 0u)
            {
                Singleton<PathManager>.instance.ReleasePath(data.m_path);
                data.m_path = 0u;
            }
            NetManager instance = Singleton<NetManager>.instance;
            if ((instance.m_nodes.m_buffer[(int)data.m_startNode].m_flags & NetNode.Flags.Ambiguous) != NetNode.Flags.None)
            {
                for (int i = 0; i < 8; i++)
                {
                    ushort segment = instance.m_nodes.m_buffer[(int)data.m_startNode].GetSegment(i);
                    if (segment != 0 && segment != segmentID && instance.m_segments.m_buffer[(int)segment].m_path != 0u)
                    {
                        return true;
                    }
                }
            }
            if ((instance.m_nodes.m_buffer[(int)data.m_endNode].m_flags & NetNode.Flags.Ambiguous) != NetNode.Flags.None)
            {
                for (int j = 0; j < 8; j++)
                {
                    ushort segment2 = instance.m_nodes.m_buffer[(int)data.m_endNode].GetSegment(j);
                    if (segment2 != 0 && segment2 != segmentID && instance.m_segments.m_buffer[(int)segment2].m_path != 0u)
                    {
                        return true;
                    }
                }
            }
            Vector3 position = instance.m_nodes.m_buffer[(int)data.m_startNode].m_position;
            Vector3 position2 = instance.m_nodes.m_buffer[(int)data.m_endNode].m_position;
            PathUnit.Position startPosA;
            PathUnit.Position startPosB;
            float num;
            float num2;
            
            if (!PathManager.FindPathPosition(position, netService, NetInfo.LaneType.Pedestrian, VehicleInfo.VehicleType.None, true, false, 32f, out startPosA, out startPosB, out num, out num2))
            {
                CustomTransportLineAI.CheckSegmentProblems(segmentID, ref data);
                return true;
            }

            PathUnit.Position endPosA;
            PathUnit.Position endPosB;
            float num3;
            float num4;

            if (!PathManager.FindPathPosition(position2, netService, NetInfo.LaneType.Pedestrian, VehicleInfo.VehicleType.None, true, false, 32f, out endPosA, out endPosB, out num3, out num4))
            {
                CustomTransportLineAI.CheckSegmentProblems(segmentID, ref data);
                return true;
            }

            if ((instance.m_nodes.m_buffer[(int)data.m_startNode].m_flags & NetNode.Flags.Fixed) != NetNode.Flags.None)
            {
                startPosB = default(PathUnit.Position);
            }
            if ((instance.m_nodes.m_buffer[(int)data.m_endNode].m_flags & NetNode.Flags.Fixed) != NetNode.Flags.None)
            {
                endPosB = default(PathUnit.Position);
            }
            startPosA.m_offset = 128;
            startPosB.m_offset = 128;
            endPosA.m_offset = 128;
            endPosB.m_offset = 128;
            bool stopLane = CustomTransportLineAI.GetStopLane(ref startPosA, vehicleType);
            bool stopLane2 = CustomTransportLineAI.GetStopLane(ref startPosB, vehicleType);
            bool stopLane3 = CustomTransportLineAI.GetStopLane(ref endPosA, vehicleType);
            bool stopLane4 = CustomTransportLineAI.GetStopLane(ref endPosB, vehicleType);
            if ((!stopLane && !stopLane2) || (!stopLane3 && !stopLane4))
            {
                CustomTransportLineAI.CheckSegmentProblems(segmentID, ref data);
                return true;
            }
            uint path;
            bool createPathResult = Singleton<PathManager>.instance.CreatePath(ExtendedVehicleType.Bus | ExtendedVehicleType.Tram, out path, ref Singleton<SimulationManager>.instance.m_randomizer, Singleton<SimulationManager>.instance.m_currentBuildIndex, startPosA, startPosB, endPosA, endPosB, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, vehicleType, 20000f, false, true, true, skipQueue);
            if (createPathResult)
            {
                if (startPosA.m_segment != 0 && startPosB.m_segment != 0)
                {
                    NetNode[] expr_2D9_cp_0 = instance.m_nodes.m_buffer;
                    ushort expr_2D9_cp_1 = data.m_startNode;
                    expr_2D9_cp_0[(int)expr_2D9_cp_1].m_flags = (expr_2D9_cp_0[(int)expr_2D9_cp_1].m_flags | NetNode.Flags.Ambiguous);
                }
                else
                {
                    NetNode[] expr_305_cp_0 = instance.m_nodes.m_buffer;
                    ushort expr_305_cp_1 = data.m_startNode;
                    expr_305_cp_0[(int)expr_305_cp_1].m_flags = (expr_305_cp_0[(int)expr_305_cp_1].m_flags & ~NetNode.Flags.Ambiguous);
                }
                if (endPosA.m_segment != 0 && endPosB.m_segment != 0)
                {
                    NetNode[] expr_344_cp_0 = instance.m_nodes.m_buffer;
                    ushort expr_344_cp_1 = data.m_endNode;
                    expr_344_cp_0[(int)expr_344_cp_1].m_flags = (expr_344_cp_0[(int)expr_344_cp_1].m_flags | NetNode.Flags.Ambiguous);
                }
                else
                {
                    NetNode[] expr_370_cp_0 = instance.m_nodes.m_buffer;
                    ushort expr_370_cp_1 = data.m_endNode;
                    expr_370_cp_0[(int)expr_370_cp_1].m_flags = (expr_370_cp_0[(int)expr_370_cp_1].m_flags & ~NetNode.Flags.Ambiguous);
                }
                data.m_path = path;
                data.m_flags |= NetSegment.Flags.WaitingPath;
                return false;
            }
            CustomTransportLineAI.CheckSegmentProblems(segmentID, ref data);
            return true;
        }

        [RedirectFrom(typeof(TransportLineAI))]
        public static new bool UpdatePath(ushort segmentID, ref NetSegment data, ItemClass.Service netService, VehicleInfo.VehicleType vehicleType, bool skipQueue)
        {
            if (data.m_path == 0u)
            {
                return StartPathFind(segmentID, ref data, netService, vehicleType, skipQueue);
            }
            if ((data.m_flags & NetSegment.Flags.WaitingPath) == NetSegment.Flags.None)
            {
                return true;
            }
            PathManager instance = Singleton<PathManager>.instance;
            NetManager instance2 = Singleton<NetManager>.instance;
            byte pathFindFlags = instance.m_pathUnits.m_buffer[(int)((UIntPtr)data.m_path)].m_pathFindFlags;
            if ((pathFindFlags & 4) != 0)
            {
                bool flag = false;
                PathUnit.Position pathPos;
                if (instance.m_pathUnits.m_buffer[(int)((UIntPtr)data.m_path)].GetPosition(0, out pathPos))
                {
                    flag = TransportLineAI.CheckNodePosition(data.m_startNode, pathPos);
                }
                if (instance.m_pathUnits.m_buffer[(int)((UIntPtr)data.m_path)].GetLastPosition(out pathPos))
                {
                    TransportLineAI.CheckNodePosition(data.m_endNode, pathPos);
                }
                float length = instance.m_pathUnits.m_buffer[(int)((UIntPtr)data.m_path)].m_length;
                if (length != data.m_averageLength)
                {
                    data.m_averageLength = length;
                    ushort transportLine = instance2.m_nodes.m_buffer[(int)data.m_startNode].m_transportLine;
                    if (transportLine != 0)
                    {
                        Singleton<TransportManager>.instance.UpdateLine(transportLine);
                    }
                }
                if (data.m_lanes != 0u)
                {
                    instance2.m_lanes.m_buffer[(int)((UIntPtr)data.m_lanes)].m_length = data.m_averageLength * ((!flag) ? 1f : 0.75f);
                }
                data.m_flags &= ~NetSegment.Flags.WaitingPath;
                data.m_flags &= ~NetSegment.Flags.PathFailed;
                data.m_flags |= NetSegment.Flags.PathLength;
                CustomTransportLineAI.CheckSegmentProblems(segmentID, ref data);
                return true;
            }
            if ((pathFindFlags & 8) != 0)
            {
                Vector3 position = instance2.m_nodes.m_buffer[(int)data.m_startNode].m_position;
                Vector3 position2 = instance2.m_nodes.m_buffer[(int)data.m_endNode].m_position;
                float num = Vector3.Distance(position, position2);
                if (num != data.m_averageLength)
                {
                    data.m_averageLength = num;
                    ushort transportLine2 = instance2.m_nodes.m_buffer[(int)data.m_startNode].m_transportLine;
                    if (transportLine2 != 0)
                    {
                        Singleton<TransportManager>.instance.UpdateLine(transportLine2);
                    }
                }
                data.m_flags &= ~NetSegment.Flags.WaitingPath;
                data.m_flags |= NetSegment.Flags.PathFailed;
                data.m_flags |= NetSegment.Flags.PathLength;
                CustomTransportLineAI.CheckSegmentProblems(segmentID, ref data);
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(TransportLineAI))]
        private static bool GetStopLane(ref PathUnit.Position pos, VehicleInfo.VehicleType vehicleType)
        {
            throw new NotImplementedException("GetStopLane is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(TransportLineAI))]
        private void RemoveLaneConnection(ushort nodeID, ref NetNode data)
        {
            throw new NotImplementedException("RemoveLaneConnection is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(TransportLineAI))]
        private void AddLaneConnection(ushort nodeID, ref NetNode data, uint laneID, byte offset)
        {
            throw new NotImplementedException("AddLaneConnection is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(TransportLineAI))]
        private static void CheckSegmentProblems(ushort segmentID, ref NetSegment data)
        {
            throw new NotImplementedException("CheckSegmentProblems is target of redirection and is not implemented.");
        }
    }
}
