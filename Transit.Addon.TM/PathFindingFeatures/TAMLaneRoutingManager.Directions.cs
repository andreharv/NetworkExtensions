using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.Traffic;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public partial class TAMLaneRoutingManager
    {
        private TAMLaneRoute TransformToRoute(uint laneId, TAMLaneDirection direction)
        {
            if (direction == TAMLaneDirection.None)
            {
                return null;
            }

            var lane = NetManager.instance.m_lanes.m_buffer[laneId];
            var segmentId = lane.m_segment;
            var segment = NetManager.instance.m_segments.m_buffer[segmentId];
            var nodeId = NetManager.instance.FindLaneNode(laneId);

            if (nodeId == null)
            {
                return null;
            }

            var segmentDir = segment.GetDirection(nodeId.Value);
            var node = NetManager.instance.m_nodes.m_buffer[nodeId.Value];
            var otherSegmentIds = node.GetSegmentIds().Except(segmentId).ToArray();
            var connections = new List<uint>();

            foreach (var otherSegmentId in otherSegmentIds)
            {
                var otherSegment = NetManager.instance.m_segments.m_buffer[segmentId];
                var otherSegmentDir = otherSegment.GetDirection(nodeId.Value);

                var relDirection = GetRelativeDirection(segmentDir, otherSegmentDir);

                if ((relDirection & direction) != 0)
                {
                    // Good direction
                    foreach (var item in otherSegment.GetL)
                    {

                    }
                }
            }

            if (!connections.Any())
            {
                return null;
            }

            return new TAMLaneRoute()
            {
                LaneId = laneId,
                NodeId = nodeId.Value,
                Connections = connections.ToArray()
            };
        }

        private TAMLaneDirection GetRelativeDirection(Vector3 source, Vector3 destination)
        {
            // TODO: Validate that
            if (Vector3.Angle(source, destination) > 150f)
            {
                return TAMLaneDirection.Forward;
            }
            else
            {
                if (Vector3.Dot(Vector3.Cross(source, -destination), Vector3.up) > 0f)
                {
                    return TAMLaneDirection.Right;
                }
                else
                {
                    return TAMLaneDirection.Left;
                }
            }
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
                uint laneFlags = (uint)Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags;
                laneFlags &= Flags.lfr; // filter arrows
                arrows = (TAMLaneDirection)laneFlags;
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
            uint laneFlags = (uint)Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags;

            if (((NetLane.Flags)laneFlags & NetLane.Flags.Created) == NetLane.Flags.None)
            {
                Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags = 0;
            }
            else
            {
                Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags &= (ushort)~Flags.lfr;
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
