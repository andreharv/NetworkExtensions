using System.Linq;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.ToolsV3.LaneRouting.Data
{
    public static partial class NodeRoutingDataExtensions
    {
        public static void UpdateArrows(this NodeRoutingData nodeRouting)
        {
            var node = NetManager.instance.GetNode(nodeRouting.NodeId);
            if (node == null)
            {
                return;
            }

            SetDefaultArrows(nodeRouting);
            SetRoutedArrows(nodeRouting);
        }

        public static void SetDefaultArrows(this NodeRoutingData nodeRouting)
        {
            var node = NetManager.instance.GetNode(nodeRouting.NodeId);
            if (node == null)
            {
                return;
            }

            foreach (var segmentId in node.Value.GetSegmentIds())
            {
                var segment = NetManager.instance.GetSegment(segmentId);
                if (segment == null)
                {
                    continue;
                }

                var segmentValue = segment.Value;
                var info = segmentValue.Info;
                info.m_netAI.UpdateLanes(segmentId, ref segmentValue, false);
            }
        }

        public static void SetRoutedArrows(this NodeRoutingData nodeRouting)
        {
            var node = NetManager.instance.GetNode(nodeRouting.NodeId);
            if (node == null)
            {
                return;
            }

            foreach (var group in nodeRouting.Routes.GroupBy(r => r.GetOriginUniqueId()))
            {
                var originSegmentId = group.First().OriginSegmentId;
                var originSegment = NetManager.instance.GetSegment(originSegmentId);
                if (originSegment == null)
                {
                    continue;
                }

                var originLaneId = group.First().OriginLaneId;
                var originLane = NetManager.instance.GetLane(LaneRoutingData.LANEROUTING_CONTROL_BIT, originLaneId);
                if (originLane == null)
                {
                    continue;
                }

                var originSegmentDirection = originSegment.Value.GetDirection(nodeRouting.NodeId);
                var originLaneArrowFlags = NetLane.Flags.None;

                foreach (var route in group)
                {
                    var destSegmentId = route.DestinationSegmentId;
                    var destSegment = NetManager.instance.GetSegment(destSegmentId);
                    if (destSegment == null)
                    {
                        continue;
                    }

                    var destSegmentDirection = destSegment.Value.GetDirection(nodeRouting.NodeId);
                    if (Vector3.Angle(originSegmentDirection, destSegmentDirection) > 150f)
                    {
                        originLaneArrowFlags |= NetLane.Flags.Forward;
                    }
                    else
                    {
                        if (Vector3.Dot(Vector3.Cross(originSegmentDirection, -destSegmentDirection), Vector3.up) > 0f)
                        {
                            originLaneArrowFlags |= NetLane.Flags.Right;
                        }
                        else
                        {
                            originLaneArrowFlags |= NetLane.Flags.Left;
                        }
                    }
                }

                if (originLaneArrowFlags != NetLane.Flags.None)
                {
                    NetManager.instance.m_lanes.m_buffer[originLaneId].m_flags =
                        (ushort)(
                            ((NetLane.Flags)originLane.Value.m_flags &
                            ~NetLane.Flags.LeftForwardRight) |
                            originLaneArrowFlags);
                }
            }
        }
    }
}
