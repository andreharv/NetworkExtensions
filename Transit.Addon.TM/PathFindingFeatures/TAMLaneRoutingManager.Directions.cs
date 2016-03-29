using ColossalFramework;
using System;
using System.Collections.Generic;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.Traffic;
using Transit.Framework;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public partial class TAMLaneRoutingManager
    {
        private TAMLaneRoute TransformToRoute(uint laneId, TAMLaneDirection direction)
        {
            var lane = NetManager.instance.m_lanes.m_buffer[laneId];
            var segment = NetManager.instance.m_segments.m_buffer[lane.m_segment];
            var nodeId = NetManager.instance.FindLaneNode(laneId);

            var similarLanes = segment.m_lanes
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

        public void ApplyAllDirections()
        {
            for (uint i = 0; i < _laneDirections.Length; ++i)
            {
                if (!ApplyLaneDirection(i))
                    _laneDirections[i] = null;
            }
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
