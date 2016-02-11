using ColossalFramework.Math;
using UnityEngine;

namespace Transit.Addon.ToolsV2
{
    public static class OverlayEffectExtensions
    {
        public static void DrawNodeSelection(this OverlayEffect effect, RenderManager.CameraInfo camera, NetNode node, Color color, float size = 15f)
        {
            effect.DrawCircle(camera, color, node.m_position, size, node.m_position.y - 1f, node.m_position.y + 1f, true, true);
        }

        public static void DrawLaneSelection(this OverlayEffect effect, RenderManager.CameraInfo camera, uint laneId, Color color, float size = 1.5f)
        {
            if (laneId == 0)
                return;

            var lane = NetManager.instance.m_lanes.m_buffer[laneId];
            Bezier3 bezier = lane.m_bezier;
            effect.DrawBezier(camera, color, bezier, size, 0, 0, Mathf.Min(bezier.a.y, bezier.d.y) - 1f, Mathf.Max(bezier.a.y, bezier.d.y) + 1f, true, true);
        }

        // TODO: rename blip
        public static void DrawBlip(this OverlayEffect effect, RenderManager.CameraInfo camera, Vector3 pos, Color color, float size = 2f)
        {
            effect.DrawCircle(camera, color, pos, size, pos.y - 1f, pos.y + 1f, true, true);
        }

        public static void DrawRouting(this OverlayEffect effect, RenderManager.CameraInfo camera, Vector3 start, Vector3 end, Vector3 middlePoint, Color color, float size)
        {
            Bezier3 bezier;
            bezier.a = start;
            bezier.d = end;
            NetSegment.CalculateMiddlePoints(bezier.a, (middlePoint - bezier.a).normalized, bezier.d, (middlePoint - bezier.d).normalized, false, false, out bezier.b, out bezier.c);

            effect.DrawBezier(camera, color, bezier, size, 0, 0, Mathf.Min(bezier.a.y, bezier.d.y) - 1f, Mathf.Max(bezier.a.y, bezier.d.y) + 1f, true, true);
        }
    }
}
