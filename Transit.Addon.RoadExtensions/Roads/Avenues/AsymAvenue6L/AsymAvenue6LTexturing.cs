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
                        if (info.m_segments[i].m_mesh.name.ToLower().Contains("ground_p"))
                        {
                            info.m_segments[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete2__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete2_LOD__AlphaMap.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__XYSMap.png"));
                        }
                        else if (lanesStyle != LanesLayoutStyle.Symmetrical)
                        {
                            var inverted = string.Empty;
                            var lodInverted = string.Empty;
                            var invertMe = (lanesStyle == LanesLayoutStyle.AsymL2R3 && ((info.m_segments[i].m_backwardForbidden & NetSegment.Flags.Invert) == 0));
                            if (invertMe)
                                inverted = "_Inverted";
                            else if (info.m_segments[i].m_mesh.name != "Bus")
                                lodInverted = "_Inverted";

                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (string.Format(@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment{0}__MainTex.png", inverted),
                                    string.Format(@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment{0}__AlphaMap.png", inverted)),
                                new LODTextureSet
                                    (string.Format(@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment{0}__MainTex_LOD.png", lodInverted),
                                    string.Format(@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment{0}__AlphaMap_LOD.png", lodInverted),
                                    @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment__XYSMap_LOD.png"));
                        }
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        var groundP = info.m_nodes[i].m_mesh.name.ToLower().Contains("ground_p");
                        if (groundP || info.m_nodes[i].m_mesh.name.Contains("MedCon"))
                        {
                            info.m_nodes[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete{(groundP ? "2" : "")}__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete{(groundP ? "2" : "")}_LOD__AlphaMap.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__XYSMap.png"));
                        }
                        else
                        {
                            info.m_nodes[i].SetTextures(
                             new TextureSet
                                (@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__MainTex.png",
                               @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__MainTex_LOD.png",
                                @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__AlphaMap_LOD.png",
                                @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment__XYSMap_LOD.png"));
                        }
                    }
                    break;
                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:
                case NetInfoVersion.GroundPavement:
                    var suffix = version == NetInfoVersion.GroundPavement ? "Concrete2" : "Grass";
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name.ToLower().Contains("ground_p"))
                        {
                            info.m_segments[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median{suffix}__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median{suffix}_LOD__AlphaMap.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__XYSMap.png"));
                        }
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        var groundP = info.m_nodes[i].m_mesh.name.ToLower().Contains("ground_p");
                        if (groundP || info.m_nodes[i].m_mesh.name.Contains("MedCon"))
                        {
                            info.m_nodes[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete{(groundP ? "2" : "")}__AlphaMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__MainTex.png",
                                $@"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_MedianConcrete{(groundP ? "2" : "")}_LOD__AlphaMap.png",
                                @"Roads\SmallRoads\BasicRoadMdn\Textures\Ground_Segment_Median_LOD__XYSMap.png"));
                        }
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
                                @"Roads\Avenues\AsymAvenue6L\Textures\Slope_Segment__MainTex_LOD.png",
                                @"Roads\Avenues\AsymAvenue6L\Textures\Slope_Segment__AlphaMap_LOD.png",
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
                    info.SetAllNodesTexture(
                         new TextureSet
                            (@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__MainTex.png",
                           @"Roads\Avenues\AsymAvenue6L\Textures\Slope_U_Node__AlphaMap.png"),
                        new LODTextureSet
                            (@"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__MainTex_LOD.png",
                            @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Node__AlphaMap_LOD.png",
                            @"Roads\Avenues\AsymAvenue6L\Textures\Ground_Segment__XYSMap_LOD.png"));
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

