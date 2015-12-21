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

        private const int LANES_XOFFSET = PAVEMENT_XOFFSET + (PAVEMENT_WIDTH - LANES_WIDTH) / 2;
        private const int TEARS_WIDTH = 120;

        private const int TEARS_XOFFSET = (LANE_WIDTH - TEARS_WIDTH) / 2;

        private const byte LINE_ALPHA = 200;
        private const byte TEARS_ALPHA = 17;

        ///////////////////////////
        // Default               //
        ///////////////////////////
        public readonly ITextureProvider Default_Segment_MainTex = TextureBlender
            .FromBaseFile(@"Roads\Common\Textures\MainTex\Base__MainTex.png", "HW2L_Default_Segment__MainTex")
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png", LANES_XOFFSET)
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Dashed__MainTex.png", LANE_WIDTH)
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png", LANE_WIDTH);

        public readonly ITextureProvider Default_Segment_APRMap = TextureBlender
            .FromBaseFile(@"Roads\Common\Textures\APRMap\Base__APRMap.png", "HW2L_Default_Segment__APRMap")
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", LANES_XOFFSET, LINE_ALPHA)
            .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", TEARS_XOFFSET, TEARS_ALPHA, false)
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Dashed__APRMap.png", LANE_WIDTH, LINE_ALPHA)
            .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", TEARS_XOFFSET, TEARS_ALPHA, false)
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", LANE_WIDTH, LINE_ALPHA);

        public readonly ITextureProvider Default_Node_MainTex = TextureBlender
            .FromBaseFile(@"Roads\Common\Textures\MainTex\Base__MainTex.png", "HW2L_Default_Node__MainTex")
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png", LANES_XOFFSET)
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png", LANE_WIDTH + LINE_WIDTH + LANE_WIDTH);

        public readonly ITextureProvider Default_Node_APRMap = TextureBlender
            .FromBaseFile(@"Roads\Common\Textures\APRMap\Base__APRMap.png", "HW2L_Default_Node__APRMap")
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", LANES_XOFFSET, LINE_ALPHA)
            .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", TEARS_XOFFSET, TEARS_ALPHA, false)
            .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", LANE_WIDTH + LINE_WIDTH + TEARS_XOFFSET, TEARS_ALPHA, false)
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", LANE_WIDTH + LINE_WIDTH + LANE_WIDTH, LINE_ALPHA);

        ///////////////////////////
        // Slope                 //
        ///////////////////////////
        public readonly ITextureProvider Slope_Segment_MainTex = TextureBlender
            .FromBaseFile(@"Roads\Common\Textures\MainTex\Base_Slope__MainTex.png", "HW2L_Slope_Segment__MainTex")
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png", LANES_XOFFSET)
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Dashed__MainTex.png", LANE_WIDTH)
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png", LANE_WIDTH);

        public readonly ITextureProvider Slope_Segment_APRMap = TextureBlender
            .FromBaseFile(@"Roads\Common\Textures\APRMap\Base_Slope__APRMap.png", "HW2L_Slope_Segment__APRMap")
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", LANES_XOFFSET, LINE_ALPHA)
            .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", TEARS_XOFFSET, TEARS_ALPHA, false)
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Dashed__APRMap.png", LANE_WIDTH, LINE_ALPHA)
            .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", TEARS_XOFFSET, TEARS_ALPHA, false)
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", LANE_WIDTH, LINE_ALPHA);

        public readonly ITextureProvider Slope_Node_MainTex = TextureBlender
            .FromBaseFile(@"Roads\Common\Textures\MainTex\Base_Slope__MainTex.png", "HW2L_SlopeNode__MainTex")
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png", LANES_XOFFSET)
            .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png", LANE_WIDTH + LINE_WIDTH + LANE_WIDTH);

        public readonly ITextureProvider Slope_Node_APRMap = TextureBlender
            .FromBaseFile(@"Roads\Common\Textures\APRMap\Base_Slope__APRMap.png", "HW2L_SlopeNode__APRMap")
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", LANES_XOFFSET, LINE_ALPHA)
            .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", TEARS_XOFFSET, TEARS_ALPHA, false)
            .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", LANE_WIDTH + LINE_WIDTH + TEARS_XOFFSET, TEARS_ALPHA, false)
            .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", LANE_WIDTH + LINE_WIDTH + LANE_WIDTH, LINE_ALPHA);
    }
}
