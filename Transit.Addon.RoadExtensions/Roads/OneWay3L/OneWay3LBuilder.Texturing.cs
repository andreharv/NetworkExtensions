using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Roads.OneWay3L
{
    public partial class OneWay3LBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                            (@"Roads\OneWay3L\Textures\Ground\Segment__MainTex.png",
                            @"Roads\OneWay3L\Textures\Ground\Segment__AlphaMap.png"),
                        new LODTexturesSet
                            (@"Roads\OneWay3L\Textures\Ground\SegmentLOD__MainTex.png",
                            @"Roads\OneWay3L\Textures\Ground\SegmentLOD__AlphaMap.png",
                            @"Roads\OneWay3L\Textures\Ground\SegmentLOD__XYS.png"));
                    break;
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                            (@"Roads\OneWay3L\Textures\ElevatedBridge\MainTex.png",
                            @"Roads\OneWay3L\Textures\ElevatedBridge\Segment__APRMap.png"),
						new LODTexturesSet
							(@"Roads\OneWay3L\Textures\ElevatedBridge\LOD__MainTex.png",
							@"Roads\OneWay3L\Textures\ElevatedBridge\SegmentLOD__APRMap.png",
							@"Roads\OneWay3L\Textures\ElevatedBridge\LOD__XYSMap.png"));
					info.SetAllNodesTexture(
                        new TexturesSet
                            (@"Roads\OneWay3L\Textures\ElevatedBridge\MainTex.png",
                            @"Roads\OneWay3L\Textures\ElevatedBridge\Node__APRMap.png"),
                        new LODTexturesSet
                            (@"Roads\OneWay3L\Textures\ElevatedBridge\LOD__MainTex.png",
                            @"Roads\OneWay3L\Textures\ElevatedBridge\NodeLOD__APRMap.png",
                            @"Roads\OneWay3L\Textures\ElevatedBridge\LOD__XYSMap.png"));
                    break;
            }
        }
    }
}

