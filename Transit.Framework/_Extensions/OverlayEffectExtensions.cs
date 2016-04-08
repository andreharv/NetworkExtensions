using ColossalFramework.Math;
using UnityEngine;

namespace Transit.Framework
{
    public static class OverlayEffectExtensions
    {
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
