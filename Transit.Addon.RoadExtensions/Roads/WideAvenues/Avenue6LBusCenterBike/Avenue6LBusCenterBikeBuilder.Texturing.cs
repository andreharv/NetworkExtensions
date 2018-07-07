using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Texturing;
#if DEBUG
using Debug = Transit.Framework.Debug;
#endif
namespace Transit.Addon.RoadExtensions.Roads.WideAvenues.Avenue6LBusCenterBike
{
    public partial class Avenue6LBusCenterBikeBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {

                case NetInfoVersion.Ground:
                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:

                    var isGrass = version.ToString().Substring(6).Length > 0;
                    var suffix = isGrass ? "Grass" : "Concrete";

                    info.m_segments[0].SetTextures(
                       new TextureSet
                           (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment__MainTex.png",
                           $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment{suffix}__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__MainTex.png",
                          $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment{suffix}_LOD__APRMap.png",
                           @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));

                    info.m_segments[1].SetTextures(
                       new TextureSet
                           (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment__MainTex1.png",
                           $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment{suffix}BeforeEnd_Inverted__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__MainTex.png",
                            $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_SegmentConcrete_LOD__APRMap.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));

                    info.m_segments[2].SetTextures(
                       new TextureSet
                           (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment__MainTex2.png",
                           $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment{suffix}BeforeEnd_Inverted__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__MainTex.png",
                          $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_SegmentConcrete_LOD__APRMap.png",
                           @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));


                    info.m_segments[3].SetTextures(
                        new TextureSet
                               (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_Bus__MainTex.png",
                               $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_SegmentConcrete_Bus__APRMap.png"),
                       new LODTextureSet
                               (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__MainTex.png",
                              $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_SegmentConcrete_LOD__APRMap.png",
                               @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));

                    info.m_segments[4].SetTextures(
                    new TextureSet
                           (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_Bus__MainTex.png",
                           $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_SegmentConcrete_Bus__APRMap.png"),
                   new LODTextureSet
                           (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__MainTex.png",
                          $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_SegmentConcrete_LOD__APRMap.png",
                           @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));

                    info.m_segments[5].SetTextures(
                   new TextureSet
                       (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_End__MainTex2.png",
                       $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment{suffix}BeforeEnd_Inverted__APRMap.png"),
                    new LODTextureSet
                       (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__MainTex.png",
                      $@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_SegmentConcrete_LOD__APRMap.png",
                       @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                         new TextureSet
                             (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Node__MainTex.png",
                             @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Node__APRMap.png"),
                         new LODTextureSet
                            (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Node_LOD__MainTex.png",
                             @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Node_LOD__APRMap.png",
                             @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Segment_LOD__XYSMap.png"));

                    break;






















                case NetInfoVersion.Elevated:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                            (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment__MainTex.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment_LOD__MainTex.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment_LOD__APRMap.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                            (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Node__MainTex.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Node__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment_LOD__MainTex.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Node_LOD__APRMap.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Bridge:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name == "Bridge_Cables")
                        {
                            info.m_segments[i].SetTextures(
                            new TextureSet
                            (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Bridge_Cables__MainTex.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Bridge_Cables__APRMap.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Bridge_Cables__XYSMap.png"),
                            new LODTextureSet
                            (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Bridge_Cables_LOD__MainTex.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Bridge_Cables_LOD__APRMap.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Bridge_Cables_LOD__XYSMap.png"));
                        }
                        else
                        {
                            info.m_segments[i].SetTextures(
                            new TextureSet
                                (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment__MainTex.png",
                                @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment_LOD__MainTex.png",
                                @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment_LOD__APRMap.png",
                                @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment_LOD__XYSMap.png"));
                        }
                    }

                    info.SetAllNodesTexture(
                        new TextureSet
                            (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Node__MainTex.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Node__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment_LOD__MainTex.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Node_LOD__APRMap.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Elevated_Segment_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Slope:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name == "medium-tunnel-slope")
                        {
                            info.m_segments[i].SetTextures(
                        new TextureSet
                            (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment__MainTex.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment_Cover__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment_LOD__MainTex.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment_LOD__APRMap.png",
                            @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment_LOD__XYSMap.png"));
            }
                        else
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment__MainTex.png",
                                    @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment_LOD__MainTex.png",
                                    @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment_LOD__APRMap.png",
                                    @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment_LOD__XYSMap.png"));
                        }
                    }

                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name == "Slope_U_Node")
                    {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment__MainTex.png",
                                    @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Tunnel_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment_LOD__MainTex.png",
                                    @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment_LOD__APRMap.png",
                                    @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment_LOD__XYSMap.png"));
            }
                        else
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Node__MainTex.png",
                                    @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Node_LOD__MainTex.png",
                                    @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Ground_Node_LOD__APRMap.png",
                                    @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Slope_Segment_LOD__XYSMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Tunnel:
                    {
                        info.SetAllSegmentsTexture(
                            new TextureSet
                                (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Tunnel_Segment__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Tunnel_Segment_LOD__MainTex.png",
                                @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Tunnel_Segment_LOD__APRMap.png",
                                @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Tunnel_Segment_LOD__XYSMap.png"));
                        info.SetAllNodesTexture(
                            new TextureSet
                                (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Tunnel_Node__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Tunnel_Segment_LOD__MainTex.png",
                                @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Tunnel_Segment_LOD__APRMap.png",
                                @"Roads\WideAvenues\Avenue6LBusCenterBike\Textures\Tunnel_Segment_LOD__XYSMap.png"));
                        break;
                    }
            }
        }
    }
}

