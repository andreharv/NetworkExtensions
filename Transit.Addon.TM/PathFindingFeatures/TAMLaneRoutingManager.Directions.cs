using ColossalFramework;
using System;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.Traffic;
using Transit.Framework;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public partial class TAMLaneRoutingManager
    {
        public TAMLaneDirection? GetLaneDirection(uint laneId)
        {
            return _laneDirections[laneId];
        }

        public TAMLaneDirection? GetHighwayLaneDirection(uint laneId)
        {
            return _highwayLaneDirections[laneId];
        }

        public void LoadLaneDirection(uint laneId, TAMLaneDirection direction)
        {
            if (!MayHaveLaneDirection(laneId))
            {
                RemoveLaneDirection(laneId);
                return;
            }

            if (_highwayLaneDirections[laneId] != null)
                return; // disallow custom lane arrows in highway rule mode

            _laneDirections[laneId] = direction;
            ApplyLaneDirection(laneId, false);
        }

        public void SetHighwayLaneDirection(uint laneId, TAMLaneDirection direction, bool check = true)
        {
            if (check && !MayHaveLaneDirection(laneId))
            {
                RemoveLaneDirection(laneId);
                return;
            }

            _highwayLaneDirections[laneId] = direction;
            ApplyLaneDirection(laneId, false);
        }

        public bool ToggleLaneDirection(uint laneId, TAMLaneDirection flags)
        {
            if (!MayHaveLaneDirection(laneId))
            {
                RemoveLaneDirection(laneId);
                return false;
            }

            if (_highwayLaneDirections[laneId] != null)
                return false; // disallow custom lane arrows in highway rule mode

            TAMLaneDirection? arrows = _laneDirections[laneId];
            if (arrows == null)
            {
                // read currently defined arrows
                uint laneFlags = (uint) Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags;
                laneFlags &= Flags.lfr; // filter arrows
                arrows = (TAMLaneDirection) laneFlags;
            }

            arrows ^= flags;
            _laneDirections[laneId] = arrows;
            ApplyLaneDirection(laneId, false);
            return true;
        }

        private bool MayHaveLaneDirection(uint laneId)
        {
            if (laneId <= 0)
                return false;
            NetManager netManager = Singleton<NetManager>.instance;
            if (((NetLane.Flags) Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags & NetLane.Flags.Created) ==
                NetLane.Flags.None)
                return false;

            ushort segmentId = netManager.m_lanes.m_buffer[laneId].m_segment;

            var dir = NetInfo.Direction.Forward;
            var dir2 = ((netManager.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Invert) ==
                        NetSegment.Flags.None)
                ? dir
                : NetInfo.InvertDirection(dir);
            var dir3 = TrafficPriority.IsLeftHandDrive() ? NetInfo.InvertDirection(dir2) : dir2;

            NetInfo segmentInfo = netManager.m_segments.m_buffer[segmentId].Info;
            uint curLaneId = netManager.m_segments.m_buffer[segmentId].m_lanes;
            int numLanes = segmentInfo.m_lanes.Length;
            int laneIndex = 0;
            int wIter = 0;
            while (laneIndex < numLanes && curLaneId != 0u)
            {
                ++wIter;
                if (wIter >= 20)
                {
                    Log.Error("Too many iterations in Flags.mayHaveLaneArrows!");
                    break;
                }

                if (curLaneId == laneId)
                {
                    NetInfo.Lane laneInfo = segmentInfo.m_lanes[laneIndex];
                    ushort nodeId = (laneInfo.m_direction == dir3)
                        ? netManager.m_segments.m_buffer[segmentId].m_endNode
                        : netManager.m_segments.m_buffer[segmentId].m_startNode;

                    if ((netManager.m_nodes.m_buffer[nodeId].m_flags & NetNode.Flags.Created) == NetNode.Flags.None)
                        return false;
                    return (netManager.m_nodes.m_buffer[nodeId].m_flags & NetNode.Flags.Junction) != NetNode.Flags.None;
                }
                curLaneId = netManager.m_lanes.m_buffer[curLaneId].m_nextLane;
                ++laneIndex;
            }
            return false;
        }

        public void ApplyAll()
        {
            for (uint i = 0; i < _laneDirections.Length; ++i)
            {
                if (!ApplyLaneDirection(i))
                    _laneDirections[i] = null;
            }
        }

        public bool ApplyLaneDirection(uint laneId, bool check = true)
        {
            if (laneId <= 0)
                return true;

            uint laneFlags = (uint) Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags;

            if (check && !MayHaveLaneDirection(laneId))
                return false;

            TAMLaneDirection? hwArrows = _highwayLaneDirections[laneId];
            TAMLaneDirection? arrows = _laneDirections[laneId];

            if (hwArrows != null)
            {
                laneFlags &= ~Flags.lfr; // remove all arrows
                laneFlags |= (uint) hwArrows; // add highway arrows
            }
            else if (arrows != null)
            {
                TAMLaneDirection flags = (TAMLaneDirection) arrows;
                laneFlags &= ~Flags.lfr; // remove all arrows
                laneFlags |= (uint) flags; // add desired arrows
            }

            //Log._Debug($"Setting lane flags @ lane {laneId}, seg. {Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_segment} to {((NetLane.Flags)laneFlags).ToString()}");
            Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags = Convert.ToUInt16(laneFlags);
            return true;
        }

        public void RemoveLaneDirection(uint laneId)
        {
            if (laneId <= 0)
                return;

            if (_highwayLaneDirections[laneId] != null)
                return; // modification of arrows in highway rule mode is forbidden

            _laneDirections[laneId] = null;
            uint laneFlags = (uint) Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags;

            if (((NetLane.Flags) laneFlags & NetLane.Flags.Created) == NetLane.Flags.None)
            {
                Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags = 0;
            }
            else
            {
                Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags &= (ushort) ~Flags.lfr;
            }
        }

        public void RemoveHighwayLaneDirectionAtSegment(ushort segmentId)
        {
            if ((Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) ==
                NetSegment.Flags.None)
                return;

            int i = 0;
            uint curLaneId = Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_lanes;

            while (i < Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].Info.m_lanes.Length &&
                   curLaneId != 0u)
            {
                RemoveHighwayLaneDirection(curLaneId);
                curLaneId = Singleton<NetManager>.instance.m_lanes.m_buffer[curLaneId].m_nextLane;
                ++i;
            } // foreach lane
        }

        public void ClearAllHighwayDirections()
        {
            for (uint i = 0; i < Singleton<NetManager>.instance.m_lanes.m_size; ++i)
            {
                _highwayLaneDirections[i] = null;
            }
        }

        public void RemoveHighwayLaneDirection(uint laneId)
        {
            _highwayLaneDirections[laneId] = null;
        }

        public void ScrubSegment(uint segmentId)
        {
            uint laneId = NetManager.instance.m_segments.m_buffer[segmentId].m_lanes;
            while (laneId != 0)
            {
                if (!ApplyLaneDirection(laneId))
                {
                    RemoveLaneDirection(laneId);
                }
                laneId = NetManager.instance.m_lanes.m_buffer[laneId].m_nextLane;
            }
        }
    }
}
