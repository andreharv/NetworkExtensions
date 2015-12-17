using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Highways.Highway5L
{
    public partial class Highway5LBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet(
                            @"Highways\Highway5L\Textures\Ground\Segment__MainTex.png",
                            @"Highways\Highway5L\Textures\Ground\Segment__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway5L\Textures\Ground\SegmentLOD__MainTex.png",
                            @"Highways\Highway5L\Textures\Ground\SegmentLOD__APRMap.png",
                            @"Highways\Highway5L\Textures\Ground\SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Highways\Highway5L\Textures\Ground\Node__MainTex.png",
                            @"Highways\Highway5L\Textures\Ground\Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway5L\Textures\Ground\NodeLOD__MainTex.png",
                            @"Highways\Highway5L\Textures\Ground\NodeLOD__APRMap.png",
                            @"Highways\Highway5L\Textures\Ground\NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetAllSegmentsTexture(
                        new TexturesSet(
                            @"Highways\Highway5L\Textures\ElevatedBridge\Segment__MainTex.png",
                            @"Highways\Highway5L\Textures\ElevatedBridge\Segment__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway5L\Textures\ElevatedBridge\SegmentLOD__MainTex.png",
                            @"Highways\Highway5L\Textures\ElevatedBridge\SegmentLOD__APRMap.png",
                            @"Highways\Highway5L\Textures\ElevatedBridge\LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Highways\Highway5L\Textures\ElevatedBridge\Node__MainTex.png",
                            @"Highways\Highway5L\Textures\ElevatedBridge\Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway5L\Textures\ElevatedBridge\NodeLOD__MainTex.png",
                            @"Highways\Highway5L\Textures\ElevatedBridge\NodeLOD__APRMap.png",
                            @"Highways\Highway5L\Textures\ElevatedBridge\LOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Highways\Highway5L\Textures\Slope\Segment__MainTex.png",
                            @"Highways\Highway5L\Textures\Slope\Segment__APRMap.png"),
                        new LODTexturesSet
                            (@"Highways\Highway5L\Textures\Slope\SegmentLOD__MainTex.png",
                            @"Highways\Highway5L\Textures\Slope\SegmentLOD__APRMap.png",
                            @"Highways\Highway5L\Textures\Slope\SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Highways\Highway6L\Textures\Tunnel\Node__MainTex.png",
                            @"Highways\Highway6L\Textures\Ground\Node__APRMap.png"),
                        new LODTexturesSet
                           (@"Highways\Highway6L\Textures\Ground\NodeLOD__MainTex.png",
                            @"Highways\Highway6L\Textures\Ground\NodeLOD__APRMap.png",
                            @"Highways\Highway6L\Textures\Ground\NodeLOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Tunnel:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"Highways\Highway5L\Textures\Tunnel\Segment__MainTex.png",
                            @"Highways\Highway5L\Textures\Tunnel\Segment__APRMap.png"));
                    //new LODTexturesSet
                    //   (@"Highways\Highway5L\Textures\Tunnel\SegmentLOD__MainTex.png",
                    //    @"Highways\Highway5L\Textures\Tunnel\SegmentLOD__APRMap.png",
                    //    @"Highways\Highway5L\Textures\Slope\NodeLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                           (@"Highways\Highway5L\Textures\Tunnel\Node__MainTex.png",
                            @"Highways\Highway5L\Textures\Tunnel\Segment__APRMap.png"));
                    //new LODTexturesSet
                    //   (@"Highways\Highway5L\Textures\Tunnel\NodeLOD__MainTex.png",
                    //    @"Highways\Highway5L\Textures\Tunnel\SegmentLOD__APRMap.png",
                    //    @"Highways\Highway5L\Textures\Slope\NodeLOD__XYSMap.png"));
                    break;
            }
        }
    }
}