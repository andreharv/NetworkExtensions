using System.Collections.Generic;
using ColossalFramework.Math;
using Transit.Addon.TrafficTools.Common;
using Transit.Addon.TrafficTools.Common.Markers;
using UnityEngine;

namespace Transit.Addon.TrafficTools.LaneRouting.Markers
{
    public class LaneRoutingMarker : NodeMarkerBase
    {
        public uint LaneId { get; private set; }
        public Vector3 Position { get; private set; }
        public bool IsOrigin { get; private set; }
        public Color Color { get; private set; }
        public ICollection<LaneRoutingMarker> Routes { get; private set; }

        public LaneRoutingMarker(ushort nodeId, uint laneId, Vector3 position, bool isOrigin, int colorId) : base(nodeId)
        {
            LaneId = laneId;
            Position = position;
            IsOrigin = isOrigin;
            IsEnabled = isOrigin;
            Color = colors[colorId];
            Routes = new HashSet<LaneRoutingMarker>();
        }

        public override void OnRendered(RenderManager.CameraInfo camera)
        {
            switch (GetState())
            {
                case UIState.Default:
                    RenderManager.instance.OverlayEffect.DrawCircle(camera, Color, Position, 1f, Position.y - 1f, Position.y + 1f, true, true);
                    break;
                case UIState.Hovered:
                    RenderManager.instance.OverlayEffect.DrawCircle(camera, new Color32(255, 255, 255, 40), Position, 1.5f, Position.y - 1f, Position.y + 1f, true, true);
                    RenderManager.instance.OverlayEffect.DrawCircle(camera, Color, Position, 1f, Position.y - 1f, Position.y + 1f, true, true);
                    break;
                case UIState.Selected:
                case UIState.Selected | UIState.Hovered:
                    RenderSelection(camera);
                    break;
            }
        }

        private void RenderSelection(RenderManager.CameraInfo camera)
        {
            RenderManager.instance.OverlayEffect.DrawCircle(camera, new Color32(0, 255, 0, 40), Position, 1.5f, Position.y - 1f, Position.y + 1f, true, true);
            RenderManager.instance.OverlayEffect.DrawCircle(camera, Color, Position, 1f, Position.y - 1f, Position.y + 1f, true, true);

            var cursorPosition = GetCursorPositionInNode();
            if (cursorPosition != null)
            {
                RenderRouting(camera, cursorPosition.Value);
            }
        }

        private void RenderRouting(RenderManager.CameraInfo cameraInfo, Vector3 endPosition)
        {
            Vector3 middlePoint = NetManager.instance.m_nodes.m_buffer[NodeId].m_position;

            Bezier3 bezier;
            bezier.a = Position;
            bezier.d = endPosition;
            NetSegment.CalculateMiddlePoints(bezier.a, (middlePoint - bezier.a).normalized, bezier.d, (middlePoint - bezier.d).normalized, false, false, out bezier.b, out bezier.c);

            RenderManager.instance.OverlayEffect.DrawBezier(cameraInfo, Color, bezier, 0.1f, 0, 0, Mathf.Min(bezier.a.y, bezier.d.y) - 1f, Mathf.Max(bezier.a.y, bezier.d.y) + 1f, true, true);
        }

        private Vector3? GetCursorPositionInNode()
        {
            ToolBase.RaycastOutput output;
            NetLaneRoutingTool.RayCastSegmentAndNode(out output);

            var nodeId = output.m_netNode;

            if (nodeId == NodeId)
            {
                return output.m_hitPos;
            }
            else
            {
                return null;
            }
        }

        private static readonly Color32[] colors =
        {
            new Color32(200, 255, 255, 255),
            Color.blue,

            new Color32(200, 200, 255, 255),
            Color.grey,

            new Color32(255, 204, 102, 255),
            new Color32(184, 134, 11, 255), // dark golden rod

            new Color32(70, 130, 180, 255), // steel blue
            new Color32(0, 191, 255, 255), // deep sky blue

            new Color32(127, 255, 0, 255), // chartreuse
            new Color32(0, 100, 0, 255), // dark green

            new Color32(255, 165, 0, 255), // orange
            new Color32(220, 20, 60, 255), // crimson

            new Color32(240, 230, 140, 255), // khaki
            new Color32(128, 128, 0, 255), // olive
            
            new Color32(0, 139, 139, 255), // dark cyan
            Color.cyan,

            new Color32(255, 0, 255, 255), // Magenta 
            new Color32(128, 0, 128, 255), // Purple

            new Color32(161, 64, 206, 255),
            new Color32(79, 251, 8, 255),
            new Color32(243, 96, 44, 255),
            new Color32(45, 106, 105, 255),
            new Color32(253, 165, 187, 255),
            new Color32(90, 131, 14, 255),
            new Color32(58, 20, 70, 255),
            new Color32(248, 246, 183, 255),
            new Color32(255, 205, 29, 255),
            new Color32(91, 50, 18, 255),
            new Color32(76, 239, 155, 255),
            new Color32(241, 25, 130, 255),
            new Color32(125, 197, 240, 255),
            new Color32(57, 102, 187, 255),
            new Color32(160, 27, 61, 255),
            new Color32(167, 251, 107, 255),
            new Color32(165, 94, 3, 255),
            new Color32(204, 18, 161, 255),
            new Color32(208, 136, 237, 255),
            new Color32(232, 211, 202, 255),
            new Color32(45, 182, 15, 255),
            new Color32(8, 40, 47, 255),
            new Color32(249, 172, 142, 255),
            new Color32(248, 99, 101, 255),
            new Color32(180, 250, 208, 255),
            new Color32(126, 25, 77, 255),
            new Color32(243, 170, 55, 255),
            new Color32(47, 69, 126, 255),
            new Color32(50, 105, 70, 255),
            new Color32(156, 49, 1, 255),
            new Color32(233, 231, 255, 255),
            new Color32(107, 146, 253, 255),
            new Color32(127, 35, 26, 255),
            new Color32(240, 94, 222, 255),
            new Color32(58, 28, 24, 255),
            new Color32(165, 179, 240, 255),
            new Color32(239, 93, 145, 255),
            new Color32(47, 110, 138, 255),
            new Color32(57, 195, 101, 255),
            new Color32(124, 88, 213, 255),
            new Color32(252, 220, 144, 255),
            new Color32(48, 106, 224, 255),
            new Color32(90, 109, 28, 255),
            new Color32(56, 179, 208, 255),
            new Color32(239, 73, 177, 255),
            new Color32(84, 60, 2, 255),
            new Color32(169, 104, 238, 255),
            new Color32(97, 201, 238, 255),
        };
    }
}
