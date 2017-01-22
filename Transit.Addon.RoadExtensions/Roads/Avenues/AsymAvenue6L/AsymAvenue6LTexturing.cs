using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.Avenues.AsymAvenue6L
{
    public static class AsymAvenue6LTexturing
    {
        public static void SetupTextures(this NetInfo info, NetInfoVersion version, LanesLayoutStyle lanesStyle)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (lanesStyle != LanesLayoutStyle.Symmetrical)
                        {
                            var inverted = string.Empty;
                            if ((lanesStyle == LanesLayoutStyle.AsymL2R4 && ((info.m_segments[i].m_backwardForbidden & NetSegment.Flags.Invert) == 0))
                                || (info.m_segments[i].m_mesh.name == "Bus"))
                            {
                                inverted = "";
                            }
                                else
                            {
                                inverted = "_Inverted";
                            }

                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (string.Format(@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment{0}__MainTex.png", inverted),
                                    string.Format(@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment{0}__AlphaMap.png", inverted)),
                                new LODTextureSet
                                    (string.Format(@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment{0}__MainTex_LOD.png", inverted),
                                    string.Format(@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment{0}__AlphaMap_LOD.png", inverted),
                                    @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment__XYSMap_LOD.png"));
                            }
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        info.m_nodes[i].SetTextures(
                                 new TextureSet
                                    (@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__MainTex.png",
                                   @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__AlphaMap.png" ),
                                new LODTextureSet
                                    (@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__MainTex_LOD.png",
                                    @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__AlphaMap_LOD.png",
                                    @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment__XYSMap_LOD.png"));
                    }
                    break;
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (lanesStyle != LanesLayoutStyle.Symmetrical)
                        {
                            info.m_segments[i].SetTextures(
                            new TextureSet
                                (@"Roads\Avenues\AsymAvenue6L\Textures\Elevated_Segment__MainTex.png",
                                @"Roads\Avenues\AsymAvenue6L\Textures\Elevated_Segment__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\Avenues\AsymAvenue6L\Textures\Elevated_Segment__MainTex_LOD.png",
                                @"Roads\Avenues\AsymAvenue6L\Textures\Elevated_Segment__AlphaMap_LOD.png",
                                @"Roads\Avenues\AsymAvenue6L\Textures\Elevated_Segment__XYSMap_LOD.png"));
                         }
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        info.m_nodes[i].SetTextures(
                                 new TextureSet
                                    (@"Roads\Avenues\AsymAvenue6L\Textures\Elevated_Node__MainTex.png",
                                   @"Roads\Avenues\AsymAvenue6L\Textures\Elevated_Node__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\Avenues\AsymAvenue6L\Textures\Elevated_Node__MainTex_LOD.png",
                                    @"Roads\Avenues\AsymAvenue6L\Textures\Elevated_Node__AlphaMap_LOD.png",
                                    @"Roads\Avenues\AsymAvenue6L\Textures\Elevated_Segment__XYSMap_LOD.png"));
                    }
                    break;
                case NetInfoVersion.Slope:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name == "Slope")
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet(
                                    @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment__MainTex.png",
                                    @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment__AlphaMap.png"),
                            new LODTextureSet(
                                @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment__MainTex_LOD.png",
                                @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment__AlphaMap_LOD.png",
                                @"Roads\Avenues\AsymAvenue6L\Textures\Slope_Segment__XYSMap_LOD.png"));
                        }
                        else
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                   (@"Roads\Avenues\LargeAvenue8LM\Textures\Slope_Segment__MainTex.png",
                                    @"Roads\Avenues\LargeAvenue8LM\Textures\Slope_Segment_Cover__APRMap.png"));
                        }
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name == "Slope_U_Node")
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__MainTex.png",
                                    @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__AlphaMap.png"));
                        }
                    }

                    break;
                case NetInfoVersion.Tunnel:
                    {
                        for (int i = 0; i < info.m_segments.Length; i++)
                        {
                            if (info.m_segments[i].m_mesh.name == "Tunnel")
                            {
                                info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Roads\Avenues\AsymAvenue6L\Textures\Tunnel_Segment__MainTex.png",
                                    @"Roads\Avenues\AsymAvenue6L\Textures\Tunnel_Segment__AlphaMap.png"));
                            }
                            else
                            {
                                info.m_segments[i].SetTextures(
                                    new TextureSet
                                    (@"Roads\Avenues\AsymAvenue6L\Textures\Tunnel_Segment__MainTex.png",
                                    @"Roads\Avenues\AsymAvenue6L\Textures\Tunnel_Segment__AlphaMap.png"));
                            }
                        }
                        info.SetAllNodesTexture(
                            new TextureSet
                                (@"Roads\Avenues\AsymAvenue6L\Textures\Tunnel_Node__MainTex.png",
                                @"Roads\Avenues\AsymAvenue6L\Textures\Tunnel_Node__AlphaMap.png"));
                        break;
                    }
            }
        }
    }
}

