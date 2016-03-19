using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Framework;
using Transit.Framework.Network;
using UnityEngine;

namespace Transit.Addon.TM.Data
{
    [Serializable]
    public class TAMLaneRoute
    {
        public uint LaneId { get; set; }
        public ushort NodeId { get; set; }
        public uint[] Connections { get; set; }

		private readonly object _lock = new object();

        public TAMLaneRoute()
        {
            Connections = new uint[0];
        }

        /// <summary>
        /// Adds a connection from m_laneId to laneId.
        /// </summary>
        /// <param name="laneId"></param>
        /// <returns></returns>
        public bool AddConnection(uint laneId) {
			try {
				Monitor.Enter(_lock);

				if (Connections.Contains(laneId))
					return false; // already connected

				// expand the array & add the lane
				var oldLaneConnections = Connections;
				Connections = new uint[Connections.Length + 1];
				Array.Copy(oldLaneConnections, Connections, oldLaneConnections.Length);
				Connections[Connections.Length - 1] = laneId;
			} finally {
				Monitor.Exit(_lock);
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
				Monitor.Enter(_lock);

				bool found = false;
				for (int i = 0; i < Connections.Length; ++i) {
					if (Connections[i] == laneId) {
						// connected lane found. shift succeeding elements to the front.
						found = true;
						for (int k = i; k < Connections.Length - 1; ++k) {
							Connections[k] = Connections[k + 1];
						}
						break;
					}
				}

				if (!found)
					return false;

				// shrink the array
				var oldLaneConnections = Connections;
				Connections = new uint[Connections.Length - 1];
				Array.Copy(oldLaneConnections, Connections, Connections.Length);
			} finally {
				Monitor.Exit(_lock);
			}

			UpdateArrows();
			return true;
		}

		public int ConnectionCount() {
			return Connections.Length;
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

			if (Connections.Length <= 0)
				return true; // default

			while (true) {
				try {
					return Connections.Contains(laneId);
				} catch (Exception e) {
					// we might get an IndexOutOfBounds here since we are not locking
#if DEBUG
						Log.Warning("ConnectsTo: " + e.ToString());
#endif
				}
			}
		}

		void VerifyConnections() {
			int startI = 0;
			while (true) {
				for (int i = startI; i < Connections.Length; ++i) {
					try {
						ushort laneFlags = NetManager.instance.m_lanes.m_buffer[Connections[i]].m_flags;
						if ((laneFlags & ((ushort)NetLane.Flags.Created)) == 0) {
							// lane invalid
							RemoveConnection(Connections[i]);
							startI = i;
							goto CONTINUE_WHILE; // lane has been deleted; continue search for invalid lanes
						}
					} catch (Exception e) {
                        // we might get an IndexOutOfBounds here since we are not locking
#if DEBUG
                        Log.Warning("ConnectsTo: " + e.ToString());
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
            NetLane lane = NetManager.instance.m_lanes.m_buffer[LaneId];
            NetSegment segment = NetManager.instance.m_segments.m_buffer[lane.m_segment];

            if ((NodeId == 0 && !FindNode(segment)) || NetManager.instance.m_nodes.m_buffer[NodeId].CountSegments() <= 2)
                return;

            if (!Connections.Any())
            {
                SetDefaultArrows(lane.m_segment, ref NetManager.instance.m_segments.m_buffer[lane.m_segment]);
                return;
            }

            NetLane.Flags flags = (NetLane.Flags)lane.m_flags;
            flags &= ~(NetLane.Flags.LeftForwardRight);

            Vector3 segDir = segment.GetDirection(NodeId);
            foreach (uint connection in Connections)
            {
                ushort seg = NetManager.instance.m_lanes.m_buffer[connection].m_segment;
                Vector3 dir = NetManager.instance.m_segments.m_buffer[seg].GetDirection(NodeId);
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

            NetManager.instance.m_lanes.m_buffer[LaneId].m_flags = (ushort)flags;
        }

        bool FindNode(NetSegment segment)
        {
            uint laneId = segment.m_lanes;
            NetInfo info = segment.Info;
            int laneCount = info.m_lanes.Length;
            int laneIndex = 0;
            for (; laneIndex < laneCount && laneId != 0; laneIndex++)
            {
                if (laneId == LaneId)
                    break;
                laneId = NetManager.instance.m_lanes.m_buffer[laneId].m_nextLane;
            }

            if (laneIndex < laneCount)
            {
                NetInfo.Direction laneDir = ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ? info.m_lanes[laneIndex].m_finalDirection : NetInfo.InvertDirection(info.m_lanes[laneIndex].m_finalDirection);

                if ((laneDir & (NetInfo.Direction.Forward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Forward)
                    NodeId = segment.m_endNode;
                else if ((laneDir & (NetInfo.Direction.Backward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Backward)
                    NodeId = segment.m_startNode;

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
                if (laneId != LaneId && 
                    TPPLaneRoutingManager.instance.GetRoute(laneId) != null && 
                    TPPLaneRoutingManager.instance.GetRoute(laneId).Connections.Any())
                    TPPLaneRoutingManager.instance.GetRoute(laneId).UpdateArrows();

                laneId = NetManager.instance.m_lanes.m_buffer[laneId].m_nextLane;
            }
        }
    }
}
