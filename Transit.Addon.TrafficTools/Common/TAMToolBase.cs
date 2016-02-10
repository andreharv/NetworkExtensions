using System.Linq;
using UnityEngine;

namespace Transit.Addon.TrafficTools.Common
{
    public abstract class TAMToolBase : ToolBase
    {
        public static ushort? GetCursorNode()
        {
            RaycastInput raycastInput = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane);
            raycastInput.m_netService.m_service = ItemClass.Service.Road;
            raycastInput.m_netService.m_subService = ItemClass.SubService.None;
            raycastInput.m_netService.m_itemLayers = ItemClass.Layer.Default;
            raycastInput.m_ignoreNodeFlags = NetNode.Flags.None;
            raycastInput.m_ignoreSegmentFlags = NetSegment.Flags.None;
            raycastInput.m_ignoreTerrain = true;

            RaycastOutput output;
            ushort nodeId;

            if (!RayCast(raycastInput, out output))
            {
                return null;
            }

            nodeId = output.m_netNode;

            if (nodeId == 0)
            {
                // Joao Farias: I tried caching the raycast input, since it always has the same properties, but it causes weird issues
                NetManager netManager = NetManager.instance;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                NetSegment seg = netManager.m_segments.m_buffer[output.m_netSegment];

                ushort[] segNodes = new ushort[] { seg.m_startNode, seg.m_endNode };
                for (int i = 0; i < segNodes.Length; i++)
                {
                    Bounds bounds = netManager.m_nodes.m_buffer[segNodes[i]].m_bounds;
                    if (bounds.IntersectRay(ray))
                    {
                        nodeId = segNodes[i];
                        break;
                    }
                }
            }

            if (nodeId == 0)
            {
                return null;
            }
            else
            {
                return nodeId;
            }
        }

        public static Vector3? GetCursorPositionInNode(ushort nodeId)
        {
            RaycastOutput output;
            RayCastSegmentAndNode(out output);

            if (nodeId == output.m_netNode)
            {
                return output.m_hitPos;
            }
            else
            {
                return null;
            }
        }

        public static bool RayCastSegmentAndNode(out RaycastOutput output)
        {
            RaycastInput input = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane);
            input.m_netService.m_service = ItemClass.Service.Road;
            input.m_netService.m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
            input.m_ignoreSegmentFlags = NetSegment.Flags.None;
            input.m_ignoreNodeFlags = NetNode.Flags.None;
            input.m_ignoreTerrain = true;

            return RayCast(input, out output);
        }

        public static bool RayCastSegmentAndNode(out ushort netSegment, out ushort netNode)
        {
            RaycastOutput output;
            if (RayCastSegmentAndNode(out output))
            {
                netSegment = output.m_netSegment;
                netNode = output.m_netNode;

                if (NetManager.instance.m_segments.m_buffer[netSegment].Info.m_lanes.FirstOrDefault(l => (l.m_vehicleType & VehicleInfo.VehicleType.Car) == VehicleInfo.VehicleType.Car) == null)
                    netSegment = 0;

                return true;
            }

            netSegment = 0;
            netNode = 0;
            return false;
        }

        //protected static bool RaycastBezier(Bezier3 bezier)
        //{
        //    Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    float angle = Vector3.Angle(bezier.a, bezier.b);
        //    if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
        //    {
        //        angle = Vector3.Angle(bezier.b, bezier.c);
        //        if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
        //        {
        //            angle = Vector3.Angle(bezier.c, bezier.d);
        //            if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
        //            {
        //                // linear bezier
        //                Bounds bounds = bezier.GetBounds();
        //                bounds.Expand(0.5f);

        //                return bounds.IntersectRay(mouseRay);
        //            }
        //        }
        //    }

        //    // split bezier in 10 parts to correctly raycast curves
        //    const int amount = 10;
        //    const float size = 1f / amount;
        //    Bezier3 tempBezier;
        //    for (int i = 0; i < amount; i++)
        //    {
        //        tempBezier = bezier.Cut(i * size, (i + 1) * size);

        //        Bounds bounds = tempBezier.GetBounds();
        //        bounds.Expand(0.5f);

        //        if (bounds.IntersectRay(mouseRay))
        //            return true;
        //    }

        //    return false;
        //}
    }
}
