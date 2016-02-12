using Transit.Addon.ToolsV2.Common;
using UnityEngine;

namespace Transit.Addon.ToolsV2.LaneRouting.Markers
{
    public class LaneAnchorMarker : UIMarkerBase
    {
        public uint LaneId { get; private set; }
        public ushort SegmentId { get; private set; }
        public Vector3 Position { get; private set; }
        public bool IsOrigin { get; private set; }
        public Color32 Color { get; private set; }

        public LaneAnchorMarker(uint laneId, ushort segmentId, Vector3 position, bool isOrigin, int? colorId)
        {
            LaneId = laneId;
            SegmentId = segmentId;
            Position = position;
            IsOrigin = isOrigin;

            if (!isOrigin)
            {
                Disable();
            }

            if (colorId != null)
            {
                Color = s_colors[colorId.Value];
            }
            else
            {
                Color = new Color32(255, 255, 255, 255);
            }
        }

        protected override void OnRendered(RenderManager.CameraInfo camera)
        {
            if (IsOrigin)
            {
                switch (UIState)
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
                        RenderManager.instance.OverlayEffect.DrawCircle(camera, new Color32(0, 255, 0, 40), Position, 1.5f, Position.y - 1f, Position.y + 1f, true, true);
                        RenderManager.instance.OverlayEffect.DrawCircle(camera, Color, Position, 1f, Position.y - 1f, Position.y + 1f, true, true);
                        break;
                }
            }
            else
            {
                switch (UIState)
                {
                    case UIState.Default:
                        RenderManager.instance.OverlayEffect.DrawCircle(camera, Color, Position, 0.3f, Position.y - 1f, Position.y + 1f, true, true);
                        RenderManager.instance.OverlayEffect.DrawCircle(camera, Color, Position, 0.5f, Position.y - 1f, Position.y + 1f, true, true);
                        break;
                    case UIState.Hovered:
                        RenderManager.instance.OverlayEffect.DrawCircle(camera, Color, Position, 0.3f, Position.y - 1f, Position.y + 1f, true, true);
                        RenderManager.instance.OverlayEffect.DrawCircle(camera, Color, Position, 0.5f, Position.y - 1f, Position.y + 1f, true, true);
                        RenderManager.instance.OverlayEffect.DrawCircle(camera, new Color32(0, 255, 0, 40), Position, 1.0f, Position.y - 1f, Position.y + 1f, true, true);
                        RenderManager.instance.OverlayEffect.DrawCircle(camera, new Color32(0, 255, 0, 40), Position, 1.5f, Position.y - 1f, Position.y + 1f, true, true);
                        break;
                }
            }
        }

        private static readonly Color32[] s_colors =
        {
            new Color32(0,   0,   220, 255),
            new Color32(200, 100, 0,   255),
            new Color32(100,   0, 200, 255),
            new Color32(200, 200, 0,   255),

            new Color32(0,   100, 200, 255),
            new Color32(0,   200, 100, 255),
            new Color32(200, 50, 0,   255),
            new Color32(100, 200, 0,   255),

            new Color32(0,   200, 200, 255),
            new Color32(0,   50, 200, 255),
            new Color32(0,   200, 50, 255),
            new Color32(50, 200, 0,   255),


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
