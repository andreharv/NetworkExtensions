using Transit.Framework;
using Transit.Framework.Texturing;

namespace TransitPlus.Addon.RoadExtensions.Roads.Avenues.LargeAvenue8L2BusLanes
{
    public partial class LargeAvenue8L2BusLanesBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:
                    var suffix = version.ToString().Substring(6).Length > 0 ? "Grass" : "Concrete";
                    info.SetAllSegmentsTexture(
                        new TextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Segment__MainTex.png",
                            $@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Segment{suffix}__APRMap.png"),
                    new LODTextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Segment_LOD__MainTex.png",
                            $@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Segment{suffix}_LOD__APRMap.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Segment_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node_LOD__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node_LOD__APRMap.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Segment_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Elevated:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__APRMap.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Node__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Node__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Node_LOD__APRMap.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Bridge:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name == "Bridge_Cables")
                        {
                            info.m_segments[i].SetTextures(
                            new TextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Bridge_Cables__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Bridge_Cables__APRMap.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Bridge_Cables__XYSMap.png"),
                            new LODTextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Bridge_Cables_LOD__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Bridge_Cables_LOD__APRMap.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Bridge_Cables_LOD__XYSMap.png"));
                        }
                        else
                        {
                            info.m_segments[i].SetTextures(
                            new TextureSet
                                (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment__MainTex.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__MainTex.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__APRMap.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__XYSMap.png"));
                        }
                    }

                    info.SetAllNodesTexture(
                        new TextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Node__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Node__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Node_LOD__APRMap.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Elevated_Segment_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Slope:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name == "medium-tunnel-slope")
                        {
                            info.m_segments[i].SetTextures(
                        new TextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_Cover__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__APRMap.png",
                            @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__XYSMap.png"));
            }
                        else
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__APRMap.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__XYSMap.png"));
                        }
                    }

                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name == "Slope_U_Node")
                    {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__APRMap.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__XYSMap.png"));
            }
                        else
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node_LOD__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Ground_Node_LOD__APRMap.png",
                                    @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Slope_Segment_LOD__XYSMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Tunnel:
                    {
                        info.SetAllSegmentsTexture(
                            new TextureSet
                                (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment_LOD__MainTex.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment_LOD__APRMap.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment_LOD__XYSMap.png"));
                        info.SetAllNodesTexture(
                            new TextureSet
                                (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Node__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment_LOD__MainTex.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment_LOD__APRMap.png",
                                @"Roads\Avenues\LargeAvenue8L2BusLanes\Textures\Tunnel_Segment_LOD__XYSMap.png"));
                        break;
                    }
            }
        }
    }
}

