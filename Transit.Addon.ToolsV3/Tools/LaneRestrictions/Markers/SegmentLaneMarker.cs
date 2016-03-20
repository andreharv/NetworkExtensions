using ColossalFramework.Math;
using UnityEngine;

namespace Transit.Addon.TrafficTools.LaneRouting.Markers
{
    class SegmentLaneMarker
    {
        public uint m_lane;
        public int m_laneIndex;
        public float m_size = 1f;
        public Bezier3 m_bezier;
        public Bounds[] m_bounds;

        public bool IntersectRay(Ray ray)
        {
            if (m_bounds == null)
                CalculateBounds();

            foreach (Bounds bounds in m_bounds)
            {
                if (bounds.IntersectRay(ray))
                    return true;
            }

            return false;
        }

        void CalculateBounds()
        {
            float angle = Vector3.Angle(m_bezier.a, m_bezier.b);
            if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
            {
                angle = Vector3.Angle(m_bezier.b, m_bezier.c);
                if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
                {
                    angle = Vector3.Angle(m_bezier.c, m_bezier.d);
                    if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
                    {
                        // linear bezier
                        Bounds bounds = m_bezier.GetBounds();
                        bounds.Expand(1f);
                        m_bounds = new Bounds[] { bounds };
                        return;
                    }
                }
            }

            // split bezier in 10 parts to correctly raycast curves
            Bezier3 bezier;
            int amount = 10;
            m_bounds = new Bounds[amount];
            float size = 1f / amount;
            for (int i = 0; i < amount; i++)
            {
                bezier = m_bezier.Cut(i * size, (i + 1) * size);

                Bounds bounds = bezier.GetBounds();
                bounds.Expand(1f);
                m_bounds[i] = bounds;
            }

        }
    }
}
