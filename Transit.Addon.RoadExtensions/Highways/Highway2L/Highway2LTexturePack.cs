using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Highways.Highway2L
{
    public class Highway2LTexturePack
    {
        private const int PAVEMENT_XOFFSET = 270;
        private const int PAVEMENT_WIDTH = 434;

        private const int LANE_WIDTH = 141;
        private const int LINE_WIDTH = 4;

        private const int LANES_WIDTH = LANE_WIDTH * 2 + LINE_WIDTH * 3;

        private const int LANE_XOFFSET = PAVEMENT_XOFFSET + (PAVEMENT_WIDTH - LANES_WIDTH) / 2;
        private const int TEARS_WIDTH = 120;

        private const int TEARS_XOFFSET = (LANE_WIDTH - TEARS_WIDTH) / 2;

        private const byte LINE_ALPHA = 200;
        private const byte TEARS_ALPHA = 17;

        public readonly ITextureProvider SegmentMainTex = TextureBlender
            .FromBaseFile(@"Roads\Common\Textures\MainTex\Segment__MainTex.png", "HW2L_Segment__MainTex")
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png", new Point(LANE_XOFFSET, 0))
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Dashed__MainTex.png", new Point(LANE_WIDTH, 0))
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png", new Point(LANE_WIDTH, 0));

        public readonly ITextureProvider SegmentAPRMap = TextureBlender
            .FromBaseFile(@"Roads\Common\Textures\APRMap\Segment__APRMap.png", "HW2L_Segment__APRMap")
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", new Point(LANE_XOFFSET, 0), LINE_ALPHA)
            .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", new Point(TEARS_XOFFSET, 0), TEARS_ALPHA, false)
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Dashed__APRMap.png", new Point(LANE_WIDTH, 0), LINE_ALPHA)
            .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", new Point(TEARS_XOFFSET, 0), TEARS_ALPHA, false)
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", new Point(LANE_WIDTH, 0), LINE_ALPHA);

        public readonly ITextureProvider NodeMainTex = TextureBlender
            .FromBaseFile(@"Roads\Common\Textures\MainTex\Segment__MainTex.png", "HW2L_Node__MainTex")
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png", new Point(LANE_XOFFSET, 0))
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png", new Point(LANE_WIDTH + LINE_WIDTH + LANE_WIDTH, 0));

        public readonly ITextureBlender NodeAPRMap = TextureBlender
            .FromBaseFile(@"Roads\Common\Textures\APRMap\Segment__APRMap.png", "HW2L_Node__APRMap")
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", new Point(LANE_XOFFSET, 0), LINE_ALPHA)
            .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", new Point(TEARS_XOFFSET, 0), TEARS_ALPHA, false)
            .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", new Point(LANE_WIDTH + LINE_WIDTH + TEARS_XOFFSET, 0), TEARS_ALPHA, false)
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", new Point(LANE_WIDTH + LINE_WIDTH + LANE_WIDTH, 0), LINE_ALPHA);
    }
}
