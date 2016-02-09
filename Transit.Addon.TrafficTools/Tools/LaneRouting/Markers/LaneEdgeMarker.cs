using System;
using UnityEngine;

namespace Transit.Addon.TrafficTools.LaneRouting.Markers
{
    public enum UIState
    {
        Default,
        Hovered,
        Selected
    }

    public class LaneEdgeMarker
    {
        public uint LaneId { get; private set; }
        public Vector3 Position { get; private set; }
        public bool IsOrigin { get; private set; }
        public UIState State { get; private set; }
        public Color Color { get; private set; }
        public FastList<LaneEdgeMarker> Connections { get; private set; }

        public LaneEdgeMarker(uint laneId, Vector3 position, bool isOrigin, int marketId)
        {
            LaneId = laneId;
            Position = position;
            IsOrigin = isOrigin;
            Color = colors[marketId];
            State = UIState.Default;
            Connections = new FastList<LaneEdgeMarker>();
        }

        public void SetState(UIState state)
        {
            State = state;
        }

        public void Render(RenderManager.CameraInfo camera)
        {
            if (IsOrigin)
            {
                switch (State)
                {
                    case UIState.Default:
                        RenderManager.instance.OverlayEffect.DrawCircle(camera, Color, Position, 1f, Position.y - 1f, Position.y + 1f, true, true);
                        break;
                    case UIState.Hovered:
                        RenderManager.instance.OverlayEffect.DrawCircle(camera, Color, Position, 2f, Position.y - 1f, Position.y + 1f, true, true);
                        break;
                    case UIState.Selected:
                        break;
                }
            }
        }


        private static readonly Color32[] colors = 
        {
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
