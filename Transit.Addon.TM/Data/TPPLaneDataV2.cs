using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Framework.Network;
using UnityEngine;

namespace Transit.Addon.TM.Data
{
    [Serializable]
    public class TPPLaneDataV2
    {
        public uint m_laneId;
        public ushort m_nodeId;
		public uint[] m_laneConnections = new uint[0];
		public ExtendedUnitType m_unitTypes = ExtendedUnitType.RoadVehicle;
        public float m_speed = 1f;
		private object m_lock = new object();

		/// <summary>
		/// Adds a connection from m_laneId to laneId.
		/// </summary>
		/// <param name="laneId"></param>
		/// <returns></returns>
		public bool AddConnection(uint laneId) {
			try {
				Monitor.Enter(m_lock);

				if (m_laneConnections.Contains(laneId))
					return false; // already connected

				// expand the array & add the lane
				var oldLaneConnections = m_laneConnections;
				m_laneConnections = new uint[m_laneConnections.Length + 1];
				Array.Copy(oldLaneConnections, m_laneConnections, oldLaneConnections.Length);
				m_laneConnections[m_laneConnections.Length - 1] = laneId;
			} finally {
				Monitor.Exit(m_lock);
			}

			UpdateArrows();
			return true;
		}

		/// <summary>
		/// Removes a connection from m_laneId to laneId.
		/// </summary>
		/// <param name="laneId"></param>
		/// <returns></returns>
		public bool RemoveConnection(uint laneId) {
			try {
				Monitor.Enter(m_lock);

				bool found = false;
				for (int i = 0; i < m_laneConnections.Length; ++i) {
					if (m_laneConnections[i] == laneId) {
						// connected lane found. shift succeeding elements to the front.
						found = true;
						for (int k = i; k < m_laneConnections.Length - 1; ++k) {
							m_laneConnections[k] = m_laneConnections[k + 1];
						}
						break;
					}
				}

				if (!found)
					return false;

				// shrink the array
				var oldLaneConnections = m_laneConnections;
				m_laneConnections = new uint[m_laneConnections.Length - 1];
				Array.Copy(oldLaneConnections, m_laneConnections, m_laneConnections.Length);
			} finally {
				Monitor.Exit(m_lock);
			}

			UpdateArrows();
			return true;
		}

		public uint[] GetConnectionsAsArray() {
			return m_laneConnections;
		}

		public int ConnectionCount() {
			return m_laneConnections.Length;
		}

		/// <summary>
		/// Determines if the lane m_laneId is connected to the lane laneId.
		/// </summary>
		/// <param name="laneId"></param>
		/// <param name="verifyConnections">if true, does a validity check for all known connected lanes</param>
		/// <returns></returns>
		public bool ConnectsTo(uint laneId, bool verifyConnections = false) {
			if (verifyConnections) // TODO find a suitable location where we can verify connections
				VerifyConnections();

			if (m_laneConnections.Length <= 0)
				return true; // default

			while (true) {
				try {
					return m_laneConnections.Contains(laneId);
				} catch (Exception e) {
					// we might get an IndexOutOfBounds here since we are not locking
#if DEBUG
						Logger.LogWarning("ConnectsTo: %s", e.ToString());
#endif
				}
			}
		}

		void VerifyConnections() {
			int startI = 0;
			while (true) {
				for (int i = startI; i < m_laneConnections.Length; ++i) {
					try {
						ushort laneFlags = NetManager.instance.m_lanes.m_buffer[m_laneConnections[i]].m_flags;
						if ((laneFlags & ((ushort)NetLane.Flags.Created)) == 0) {
							// lane invalid
							RemoveConnection(m_laneConnections[i]);
							startI = i;
							goto CONTINUE_WHILE; // lane has been deleted; continue search for invalid lanes
						}
					} catch (Exception e) {
						// we might get an IndexOutOfBounds here since we are not locking
#if DEBUG
							Logger.LogWarning("ConnectsTo: %s", e.ToString());
#endif
					}
				}
				break; // no more lanes or everything ok
				CONTINUE_WHILE:;
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
                if (laneId != m_laneId && TPPLaneDataManager.sm_lanes[laneId] != null && TPPLaneDataManager.sm_lanes[laneId].ConnectionCount() > 0)
                    TPPLaneDataManager.sm_lanes[laneId].UpdateArrows();

                laneId = NetManager.instance.m_lanes.m_buffer[laneId].m_nextLane;
            }
        }
    }
}
