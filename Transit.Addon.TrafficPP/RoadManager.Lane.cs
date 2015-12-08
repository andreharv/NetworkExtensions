using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Transit.Addon.TrafficPP
{
    public static partial class RoadManager
    {
        [Serializable]
        public class Lane
        {
            public const ushort CONTROL_BIT = 2048;

            public uint m_laneId;
            public ushort m_nodeId;
            private List<uint> m_laneConnections = new List<uint>();
            public VehicleType m_vehicleTypes = VehicleType.All;
            public float m_speed = 1f;            

            public bool AddConnection(uint laneId)
            {
                bool exists = false;
                while (!Monitor.TryEnter(this.m_laneConnections, SimulationManager.SYNCHRONIZE_TIMEOUT))
                {
                }
                try
                {
                    if (m_laneConnections.Contains(laneId))
                        exists = true;
                    else
                        m_laneConnections.Add(laneId);
                }
                finally
                {
                    Monitor.Exit(this.m_laneConnections);
                }

                if (exists)
                    return false;

                UpdateArrows();

                return true;
            }

            public bool RemoveConnection(uint laneId)
            {
                bool result = false;
                while (!Monitor.TryEnter(this.m_laneConnections, SimulationManager.SYNCHRONIZE_TIMEOUT))
                {
                }
                try
                {
                    result = m_laneConnections.Remove(laneId);
                }
                finally
                {
                    Monitor.Exit(this.m_laneConnections);
                }

                if (result)
                    UpdateArrows();

                return result;
            }

            public uint[] GetConnectionsAsArray()
            {
                uint[] connections = null;
                while (!Monitor.TryEnter(this.m_laneConnections, SimulationManager.SYNCHRONIZE_TIMEOUT))
                {
                }
                try
                {
                    connections = m_laneConnections.ToArray();
                }
                finally
                {
                    Monitor.Exit(this.m_laneConnections);
                }
                return connections;
            }

            public int ConnectionCount()
            {
                int count = 0;
                while (!Monitor.TryEnter(this.m_laneConnections, SimulationManager.SYNCHRONIZE_TIMEOUT))
                {
                }
                try
                {
                    count = m_laneConnections.Count();
                }
                finally
                {
                    Monitor.Exit(this.m_laneConnections);
                }
                return count;
            }

            public bool ConnectsTo(uint laneId)
            {
                VerifyConnections();

                bool result = true;
                while (!Monitor.TryEnter(this.m_laneConnections, SimulationManager.SYNCHRONIZE_TIMEOUT))
                {
                }
                try
                {
                    result = m_laneConnections.Count == 0 || m_laneConnections.Contains(laneId);
                }
                finally
                {
                    Monitor.Exit(this.m_laneConnections);
                }

                return result;
            }

            void VerifyConnections()
            {
                uint[] connections = GetConnectionsAsArray();
                while (!Monitor.TryEnter(this.m_laneConnections, SimulationManager.SYNCHRONIZE_TIMEOUT))
                {
                }
                try
                {
                    foreach (uint laneId in connections)
                    {
                        NetLane lane = NetManager.instance.m_lanes.m_buffer[laneId];
                        if ((lane.m_flags & CONTROL_BIT) != CONTROL_BIT)
                            m_laneConnections.Remove(laneId);
                    }
                }
                finally
                {
                    Monitor.Exit(this.m_laneConnections);
                }
            }

            public void UpdateArrows()
            {
                VerifyConnections();
                NetLane lane = NetManager.instance.m_lanes.m_buffer[m_laneId];
                NetSegment segment = NetManager.instance.m_segments.m_buffer[lane.m_segment];

                if ((m_nodeId == 0 && !FindNode(segment)) || NetManager.instance.m_nodes.m_buffer[m_nodeId].CountSegments() <= 2)
                    return;

                if (ConnectionCount() == 0)
                {
                    SetDefaultArrows(lane.m_segment, ref NetManager.instance.m_segments.m_buffer[lane.m_segment]);
                    return;
                }

                NetLane.Flags flags = (NetLane.Flags)lane.m_flags;
                flags &= ~(NetLane.Flags.LeftForwardRight);

                Vector3 segDir = segment.GetDirection(m_nodeId);
                uint[] connections = GetConnectionsAsArray();
                foreach (uint connection in connections)
                {
                    ushort seg = NetManager.instance.m_lanes.m_buffer[connection].m_segment;
                    Vector3 dir = NetManager.instance.m_segments.m_buffer[seg].GetDirection(m_nodeId);
                    if (Vector3.Angle(segDir, dir) > 150f)
                    {
                        flags |= NetLane.Flags.Forward;
                    }
                    else 
                    {
                        
                        if (Vector3.Dot(Vector3.Cross(segDir, -dir), Vector3.up) > 0f)
                            flags |= NetLane.Flags.Right;
                        else
                            flags |= NetLane.Flags.Left;
                    }
                }

                NetManager.instance.m_lanes.m_buffer[m_laneId].m_flags = (ushort)flags;
            }

            bool FindNode(NetSegment segment)
            {
                uint laneId = segment.m_lanes;
                NetInfo info = segment.Info;
                int laneCount = info.m_lanes.Length;
                int laneIndex = 0;
                for (; laneIndex < laneCount && laneId != 0; laneIndex++)
                {
                    if (laneId == m_laneId)
                        break;
                    laneId = NetManager.instance.m_lanes.m_buffer[laneId].m_nextLane;
                }

                if (laneIndex < laneCount)
                {
                    NetInfo.Direction laneDir = ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ? info.m_lanes[laneIndex].m_finalDirection : NetInfo.InvertDirection(info.m_lanes[laneIndex].m_finalDirection);

                    if ((laneDir & (NetInfo.Direction.Forward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Forward)
                        m_nodeId = segment.m_endNode;
                    else if ((laneDir & (NetInfo.Direction.Backward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Backward)
                        m_nodeId = segment.m_startNode;
                    
                    return true;
                }

                return false;
            }

            void SetDefaultArrows(ushort seg, ref NetSegment segment)
            {
                NetInfo info = segment.Info;
                info.m_netAI.UpdateLanes(seg, ref segment, false);

                uint laneId = segment.m_lanes;
                int laneCount = info.m_lanes.Length;
                for (int laneIndex = 0; laneIndex < laneCount && laneId != 0; laneIndex++)
                {
                    if (laneId != m_laneId && RoadManager.sm_lanes[laneId] != null && RoadManager.sm_lanes[laneId].ConnectionCount() > 0)
                        RoadManager.sm_lanes[laneId].UpdateArrows();

                    laneId = NetManager.instance.m_lanes.m_buffer[laneId].m_nextLane;
                }
            }
        }

        static Lane[] sm_lanes = new Lane[NetManager.MAX_LANE_COUNT];

        public static Lane CreateLane(uint laneId)
        {
            Lane lane = new Lane()
            {
                m_laneId = laneId
            };

            NetSegment segment = NetManager.instance.m_segments.m_buffer[NetManager.instance.m_lanes.m_buffer[laneId].m_segment];
            NetInfo netInfo = segment.Info;
            int laneCount = netInfo.m_lanes.Length;
            int laneIndex = 0;
            for (uint l = segment.m_lanes; laneIndex < laneCount && l != 0; laneIndex++)
            {
                if (l == laneId)
                    break;

                l = NetManager.instance.m_lanes.m_buffer[l].m_nextLane;
            }

            if (laneIndex < laneCount)
            {
                NetInfoLane netInfoLane = netInfo.m_lanes[laneIndex] as NetInfoLane;
                if (netInfoLane != null)
                    lane.m_vehicleTypes = netInfoLane.m_allowedVehicleTypes;

                lane.m_speed = netInfo.m_lanes[laneIndex].m_speedLimit;
            }

            NetManager.instance.m_lanes.m_buffer[laneId].m_flags |= Lane.CONTROL_BIT;

            sm_lanes[laneId] = lane;

            return lane;
        }

        public static Lane GetLane(uint laneId)
        {
            Lane lane = sm_lanes[laneId];
            if (lane == null || (NetManager.instance.m_lanes.m_buffer[laneId].m_flags & Lane.CONTROL_BIT) == 0)
                lane = CreateLane(laneId);

            return lane;
        }
    }
}
