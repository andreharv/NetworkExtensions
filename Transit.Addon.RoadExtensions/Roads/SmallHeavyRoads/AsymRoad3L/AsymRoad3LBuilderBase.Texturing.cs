using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.SmallHeavyRoads.AsymRoad3L
{
    public partial class AsymRoad3LBuilderBase
    {
        private void SetupTextures(NetInfo info, NetInfoVersion version, LanesLayoutStyle lanesLayoutStyle)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    foreach (var segment in info.m_segments)
                    {
                        var inverted = string.Empty;
                        if ((lanesLayoutStyle == LanesLayoutStyle.AsymL1R2 && ((segment.m_backwardForbidden & NetSegment.Flags.Invert) == 0))
                            || (lanesLayoutStyle == LanesLayoutStyle.AsymL2R1 && ((segment.m_backwardForbidden & NetSegment.Flags.Invert) != 0)))
                        {
                            inverted = "_Inverted";
                        }

                        segment.SetTextures(
                            new TextureSet(
                                string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_Segment{0}__MainTex.png", inverted),
                                string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_Segment{0}__AlphaMap.png", inverted)),
                            new LODTextureSet(
                                string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_SegmentLOD{0}__MainTex.png", inverted),
                                string.Format(@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_SegmentLOD{0}__AlphaMap.png", inverted),
                                @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Ground_SegmentLOD__XYS.png"));
                    }
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    foreach (var segment in info.m_segments)
                    {
                        if (segment.m_mesh.name == "Elevated" || segment.m_mesh.name == "Bridge")
                        {
                            var inverted = (lanesLayoutStyle == LanesLayoutStyle.AsymL1R2 ? "_Inverted" : string.Empty);
                            segment.SetTextures(
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
                            segment.SetTextures(
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
                    foreach (var segment in info.m_segments)
                    {
                        if (segment.m_mesh.name == "Slope")
                        {
                            var inverted = (lanesLayoutStyle == LanesLayoutStyle.AsymL1R2 ? "_Inverted" : string.Empty);
                            segment.SetTextures(
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
                            segment.SetTextures(
                                new TextureSet
                                    (@"Roads\Highways\Highway4L\Textures\Slope_Segment__MainTex.png",
                                        @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Slope_Segment__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Slope_SegmentLOD__MainTex.png",
                                        @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Slope_SegmentLOD__APRMap.png",
                                        @"Roads\SmallHeavyRoads\AsymRoad3L\Textures\Slope_SegmentLOD__XYSMap.png"));
                        }
                    }

                    foreach (NetInfo.Node node in info.m_nodes)
                    {
                        if (node.m_mesh.name == "Slope_Node")
                        {
                            node.SetTextures(
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
                            node.SetTextures(
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
                        foreach (var segment in info.m_segments)
                        {
                            if (segment.m_mesh.name == "Tunnel")
                            {
                                var inverted = (lanesLayoutStyle == LanesLayoutStyle.AsymL1R2 ? "_Inverted" : string.Empty);
                                segment.SetTextures(
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

