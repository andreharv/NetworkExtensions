using System.Linq;
using UnityEngine;

namespace Transit.Framework
{
    public class ExtendedToolBase : ToolBase
    {
        public static ushort? RayCastNodeWithMoreThanOneSegment()
        {
            var node = RayCastNode();

            if (node == null)
            {
                return null;
            }

            if (NetManager.instance.m_nodes.m_buffer[node.Value].CountSegments() < 2)
            {
                return null;
            }

            return node;
        }

        public static ushort? RayCastNode()
        {
            ushort? hoveredSegment;
            ushort? hoveredNode;

            if (!RayCastSegmentAndNode(out hoveredSegment, out hoveredNode))
            {
                return null;
            }

            if (hoveredNode != null)
            {
                return hoveredNode;
            }

            if (hoveredSegment != null)
            {
                // Trying to get the hoveredNode with the closest hoveredSegment

                var segment = NetManager.instance.m_segments.m_buffer[hoveredSegment.Value];
                var startNode = NetManager.instance.m_nodes.m_buffer[segment.m_startNode];
                var endNode = NetManager.instance.m_nodes.m_buffer[segment.m_endNode];
                var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (startNode.CountSegments() > 1)
                {
                    var bounds = startNode.m_bounds;
                    bounds.extents /= 2f;
                    if (bounds.IntersectRay(mouseRay))
                    {
                        return segment.m_startNode;
                    }
                }

                if (endNode.CountSegments() > 1)
                {
                    var bounds = endNode.m_bounds;
                    bounds.extents /= 2f;
                    if (bounds.IntersectRay(mouseRay))
                    {
                        return segment.m_endNode;
                    }
                }
            }

            return null;
        }

        public static ushort? RayCastSegment()
        {
            ushort? hoveredSegment;
            ushort? hoveredNode;

            if (!RayCastSegmentAndNode(out hoveredSegment, out hoveredNode))
            {
                return null;
            }

            return hoveredSegment;
        }

        public static bool RayCastSegmentAndNode(out RaycastOutput output)
        {
            RaycastInput input = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane);
            input.m_netService.m_service = ItemClass.Service.Road;
            input.m_netService.m_itemLayers = ItemClass.Layer.Default;
            input.m_ignoreSegmentFlags = NetSegment.Flags.None;
            input.m_ignoreNodeFlags = NetNode.Flags.None;
            input.m_ignoreTerrain = true;

            return RayCast(input, out output);
        }

        public static bool RayCastSegmentAndNode(out ushort? netSegment, out ushort? netNode)
        {
            RaycastOutput output;
            if (RayCastSegmentAndNode(out output))
            {
                netSegment = output.m_netSegment;
                netNode = output.m_netNode;

                if (netSegment == 0)
                {
                    netSegment = null;
                    return true;
                }

                if (NetManager.instance.m_segments.m_buffer[netSegment.Value].Info.m_lanes.FirstOrDefault(l => (l.m_vehicleType & VehicleInfo.VehicleType.Car) == VehicleInfo.VehicleType.Car) == null)
                    netSegment = null;

                return true;
            }

            netSegment = null;
            netNode = null;
            return false;
        }
    }
}