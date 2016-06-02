using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.SmallHeavyRoads.AsymRoad3L
{
    public static class AsymRoad3LTexturing
    {
        public static void SetupTextures(this NetInfo info, NetInfoVersion version, AsymLaneType asymLaneType)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (asymLaneType != AsymLaneType.L0R0)
                        {
                            var inverted = string.Empty;
                            if ((asymLaneType == AsymLaneType.L1R2 && ((info.m_segments[i].m_backwardForbidden & NetSegment.Flags.Invert) == 0))
                                || (asymLaneType == AsymLaneType.L2R1 && ((info.m_segments[i].m_backwardForbidden & NetSegment.Flags.Invert) != 0)))
                            {
                                inverted = "_Inverted";
                            }

                            info.m_segments[i].SetTextures(
                                new TextureSet(
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_Segment{0}__MainTex.png", inverted),
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_Segment{0}__AlphaMap.png", inverted)),
                                new LODTextureSet(
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_SegmentLOD{0}__MainTex.png", inverted),
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_SegmentLOD{0}__AlphaMap.png", inverted),
                                    @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_SegmentLOD__XYS.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name == "Elevated" || info.m_segments[i].m_mesh.name == "Bridge")
                        {
                            var inverted = (asymLaneType == AsymLaneType.L1R2 ? "_Inverted" : string.Empty);
                            info.m_segments[i].SetTextures(
                            new TextureSet
                                (string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated{0}__MainTex.png", inverted),
                                string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated_Segment{0}__APRMap.png", inverted)),
                            new LODTextureSet
                                (string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated_LOD{0}__MainTex.png", inverted),
                                string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated_SegmentLOD{0}__APRMap.png", inverted),
                                @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated_LOD__XYSMap.png"));
                        }
                        else
                        {
                            info.m_segments[i].SetTextures(
                            new TextureSet
                                (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated__MainTex.png",
                                @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated_Segment__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated_LOD__MainTex.png",
                                @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated_SegmentLOD__APRMap.png",
                                @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated_LOD__XYSMap.png"));
                        }
                    }

                    info.SetAllNodesTexture(
                        new TextureSet
                            (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated__MainTex.png",
                            @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated_Node__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated_LOD__MainTex.png",
                            @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated_NodeLOD__APRMap.png",
                            @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Elevated_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Slope:
                    for (int i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name == "Slope")
                        {
                            var inverted = (asymLaneType == AsymLaneType.L1R2 ? "_Inverted" : string.Empty);
                            info.m_segments[i].SetTextures(
                                new TextureSet(
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_Segment{0}__MainTex.png", inverted),
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_Segment{0}__AlphaMap.png", inverted)),
                                new LODTextureSet(
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_SegmentLOD{0}__MainTex.png", inverted),
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_SegmentLOD{0}__AlphaMap.png", inverted),
                                    @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Slope_SegmentLOD2__XYSMap.png"));
                        }
                        else
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                   (@"Roads\Highways\Highway4L\Textures\Slope_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Slope_Segment__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Slope_SegmentLOD__MainTex.png",
                                     @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Slope_SegmentLOD__APRMap.png",
                                     @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Slope_SegmentLOD__XYSMap.png"));
                        }
                    }
                    for (int i = 0; i < info.m_nodes.Length; i++)
                    {
                        if (info.m_nodes[i].m_mesh.name == "Slope_Node")
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_Node__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_SegmentLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_SegmentLOD__AlphaMap.png",
                                    @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_SegmentLOD__XYS.png"));
                        }
                        else
                        {
                            info.m_nodes[i].SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_Segment__MainTex.png",
                                    @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_NodeLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_LOD__APRMap.png",
                                    @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_LOD__XYSMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Tunnel:
                    {
                        for (int i = 0; i < info.m_segments.Length; i++)
                        {
                            if (info.m_segments[i].m_mesh.name == "Tunnel")
                            {
                                var inverted = (asymLaneType == AsymLaneType.L1R2 ? "_Inverted" : string.Empty);
                                info.m_segments[i].SetTextures(
                                new TextureSet
                                    (string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_Segment{0}__MainTex.png", inverted),
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_Segment{0}__APRMap.png", inverted)),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_NodeLOD__MainTex.png",
                                    @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_LOD__APRMap.png",
                                    @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_LOD__XYSMap.png"));
                            }
                            else
                            {
                                info.SetAllSegmentsTexture(
                            new TextureSet
                                (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_Segment__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_NodeLOD__MainTex.png",
                                @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_LOD__APRMap.png",
                                @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_LOD__XYSMap.png"));
                            }
                        }
                        info.SetAllNodesTexture(
                            new TextureSet
                                (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_Segment__MainTex.png",
                                @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_Node__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_NodeLOD__MainTex.png",
                                @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_LOD__APRMap.png",
                                @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Tunnel_LOD__XYSMap.png"));
                        break;
                    }
            }
        }
    }
}

