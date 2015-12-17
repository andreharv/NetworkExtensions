using System.Drawing;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Highways.Highway2L
{
    public static class Highway2LTextures
    {
        private const int laneStart = 340;
        private const int laneWidth = 141;
        private const byte lineAlpha = 35;
        private const byte tearsAlpha = 15;

        public static ITextureBlender SegmentMainTex
        {
            get
            {
                return TextureBlender
                    .FromBaseFile(@"Roads\Common\Textures\MainTex\Segment__MainTex.png")
                    .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png", new Point(laneStart, 0))
                    .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Dashed__MainTex.png", new Point(laneWidth, 0))
                    .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png", new Point(laneWidth, 0));
            }
        }

        public static ITextureBlender SegmentAPRMap
        {
            get
            {
                return TextureBlender
                    .FromBaseFile(@"Roads\Common\Textures\APRMap\Segment__APRMap.png")
                    .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", new Point(laneStart, 0), lineAlpha)
                    .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", new Point(1, 0), tearsAlpha)
                    .WithComponent(@"Roads\Common\Textures\APRMap\Line_Dashed__APRMap.png", new Point(0, 0), lineAlpha)
                    .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", new Point(0, 0), tearsAlpha)
                    .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", new Point(2, 0), lineAlpha);
            }
        }

        public static ITextureBlender NodeMainTex
        {
            get
            {
                return TextureBlender
                    .FromBaseFile(@"Roads\Common\Textures\MainTex\Segment__MainTex.png")
                    .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png", new Point(laneStart, 0))
                    .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png", new Point(laneWidth + 4 + laneWidth, 0));
            }
        }

        public static ITextureBlender NodeAPRMap
        {
            get
            {
                return TextureBlender
                    .FromBaseFile(@"Roads\Common\Textures\APRMap\Segment__APRMap.png")
                    .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", new Point(laneStart, 0), lineAlpha)
                    .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", new Point(1, 0), tearsAlpha)
                    .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", new Point(4, 0), tearsAlpha)
                    .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", new Point(2, 0), lineAlpha);
            }
        }
    }
}
