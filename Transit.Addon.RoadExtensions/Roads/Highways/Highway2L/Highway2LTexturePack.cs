using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.Highways.Highway2L
{
    public class Highway2LTexturePack
    {
        private const int LANES_NB = 2;
        private const int LINES_NB = LANES_NB + 1;

        private const int PAVEMENT_XOFFSET = 270;
        private const int PAVEMENT_WIDTH = 434;

        private const int LANE_WIDTH = 141;
        private const int LINE_WIDTH = 4;

        private const int LANES_WIDTH = LANE_WIDTH * LANES_NB + LINE_WIDTH * LINES_NB;

        private const int LANES_XOFFSET = (PAVEMENT_WIDTH - LANES_WIDTH) / 2;
        private const int TEARS_WIDTH = 120;

        private const int TEARS_XOFFSET = (LANE_WIDTH - TEARS_WIDTH) / 2;

        private const byte LINE_ALPHA = 200;
        private const byte TEARS_ALPHA = 17;

        public readonly ITextureProvider Default_Segment_MainTex;
        public readonly ITextureProvider Default_Segment_APRMap;
        public readonly ITextureProvider Default_Node_MainTex;
        public readonly ITextureProvider Default_Node_APRMap;

        public readonly ITextureProvider Slope_Segment_MainTex;
        public readonly ITextureProvider Slope_Segment_APRMap;
        public readonly ITextureProvider Slope_Node_MainTex;
        public readonly ITextureProvider Slope_Node_APRMap;

        public Highway2LTexturePack()
        {
            ///////////////////////////
            // Default               //
            ///////////////////////////
            Default_Segment_MainTex = TextureBlender
                .FromBaseFile(@"Roads\Common\Textures\MainTex\Base__MainTex.png", "HW2L_Default_Segment__MainTex")
                .WithXOffset(PAVEMENT_XOFFSET)
                .WithXOffset(LANES_XOFFSET)
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png")
                .WithXOffset(LANE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Dashed__MainTex.png")
                .WithXOffset(LANE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png");

            Default_Segment_APRMap = TextureBlender
                .FromBaseFile(@"Roads\Common\Textures\APRMap\Base__APRMap.png", "HW2L_Default_Segment__APRMap")
                .WithXOffset(PAVEMENT_XOFFSET)
                .WithXOffset(LANES_XOFFSET)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", LINE_ALPHA)
                .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", TEARS_ALPHA, TEARS_XOFFSET, false)
                .WithXOffset(LANE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Dashed__APRMap.png", LINE_ALPHA)
                .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", TEARS_ALPHA, TEARS_XOFFSET, false)
                .WithXOffset(LANE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", LINE_ALPHA);

            Default_Node_MainTex = TextureBlender
                .FromBaseFile(@"Roads\Common\Textures\MainTex\Base__MainTex.png", "HW2L_Default_Node__MainTex")
                .WithXOffset(PAVEMENT_XOFFSET)
                .WithXOffset(LANES_XOFFSET)
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png")
                .WithXOffset(LANE_WIDTH)
                .WithXOffset(LINE_WIDTH)
                .WithXOffset(LANE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png");

            Default_Node_APRMap = TextureBlender
                .FromBaseFile(@"Roads\Common\Textures\APRMap\Base__APRMap.png", "HW2L_Default_Node__APRMap")
                .WithXOffset(PAVEMENT_XOFFSET)
                .WithXOffset(LANES_XOFFSET)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", LINE_ALPHA)
                .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", TEARS_ALPHA, TEARS_XOFFSET, false)
                .WithXOffset(LANE_WIDTH)
                .WithXOffset(LINE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", TEARS_ALPHA, TEARS_XOFFSET, false)
                .WithXOffset(LANE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", LINE_ALPHA);

            ///////////////////////////
            // Slope                 //
            ///////////////////////////
            Slope_Segment_MainTex = TextureBlender
                .FromBaseFile(@"Roads\Common\Textures\MainTex\Base_Slope__MainTex.png", "HW2L_Slope_Segment__MainTex")
                .WithXOffset(PAVEMENT_XOFFSET)
                .WithXOffset(LANES_XOFFSET)
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png")
                .WithXOffset(LANE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Dashed__MainTex.png")
                .WithXOffset(LANE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid__MainTex.png");

            Slope_Segment_APRMap = TextureBlender
                .FromBaseFile(@"Roads\Common\Textures\APRMap\Base_Slope__APRMap.png", "HW2L_Slope_Segment__APRMap")
                .WithXOffset(PAVEMENT_XOFFSET)
                .WithXOffset(LANES_XOFFSET)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", LINE_ALPHA)
                .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", TEARS_ALPHA, TEARS_XOFFSET, false)
                .WithXOffset(LANE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Dashed__APRMap.png", LINE_ALPHA)
                .WithComponent(@"Roads\Common\Textures\APRMap\Tearing__APRMap.png", TEARS_ALPHA, TEARS_XOFFSET, false)
                .WithXOffset(LANE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid__APRMap.png", LINE_ALPHA);

            Slope_Node_MainTex = TextureBlender
                .FromBaseFile(@"Roads\Common\Textures\MainTex\Base_Slope__MainTex.png", "HW2L_Slope_Node__MainTex")
                .WithXOffset(PAVEMENT_XOFFSET)
                .WithXOffset(LANES_XOFFSET)
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png")
                .WithXOffset(LANE_WIDTH)
                .WithXOffset(LINE_WIDTH)
                .WithXOffset(LANE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\MainTex\Line_White_Solid_Fadeout__MainTex.png");

            Slope_Node_APRMap = TextureBlender
                .FromBaseFile(@"Roads\Common\Textures\APRMap\Base_Slope__APRMap.png", "HW2L_Slope_Node__APRMap")
                .WithXOffset(PAVEMENT_XOFFSET)
                .WithXOffset(LANES_XOFFSET)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", LINE_ALPHA)
                .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", TEARS_ALPHA, TEARS_XOFFSET, false)
                .WithXOffset(LANE_WIDTH)
                .WithXOffset(LINE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\APRMap\Tearing_Fadeout__APRMap.png", TEARS_ALPHA, TEARS_XOFFSET, false)
                .WithXOffset(LANE_WIDTH)
                .WithComponent(@"Roads\Common\Textures\APRMap\Line_Solid_Fadeout__APRMap.png", LINE_ALPHA);
        }
    }
}
