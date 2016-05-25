using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.GravelTiny
{
    public partial class ZonablePedestrianTinyGravelRoadBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    info.SetAllSegmentsTexture(
                        new TextureSet(
                            @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment__MainTex.png",
                            @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment__AlphaMap.png",
                            @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment__XYSMap.png"),
                        new LODTextureSet(
                            @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment_LOD__MainTex.png",
                            @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment_LOD__AlphaMap.png",
                            @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment_LOD__XYSMap.png")
                        );

                    foreach (var node in info.m_nodes)
                    {
                        if (node.m_flagsRequired != NetNode.Flags.Transition)
                        {
                            node.SetTextures(
                                new TextureSet(
                                    @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment__MainTex.png",
                                    @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment__AlphaMap.png",
                                    @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment__XYSMap.png"),
                                new LODTextureSet(
                                    @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment_LOD__MainTex.png",
                                    @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment_LOD__AlphaMap.png",
                                    @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment_LOD__XYSMap.png")
                                );
                        }
                        else
                        {
                            node.SetTextures(
                                new TextureSet
                                    (@"Roads\PedestrianRoads\Common\Textures\Elevated_Trans__MainTex.png",
                                    @"Roads\PedestrianRoads\PavementTiny\Textures\Elevated_Trans__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\PedestrianRoads\Stone\Textures\Elevated_Trans_LOD__MainTex.png",
                                    @"Roads\PedestrianRoads\Stone\Textures\Elevated_Trans_LOD__AlphaMap.png",
                                    @"Roads\PedestrianRoads\PavementTiny\Textures\Elevated_Segment_LOD__XYSMap.png")
                                );
                        }
                    }
                    break;
            }
        }
    }
}
