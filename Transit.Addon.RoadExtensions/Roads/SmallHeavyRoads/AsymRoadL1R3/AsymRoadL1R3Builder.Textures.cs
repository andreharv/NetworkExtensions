using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.SmallHeavyRoads.AsymRoadL1R3
{
    public partial class AsymRoadL1R3Builder
    {
        private void SetupTextures(NetInfo info, NetInfoVersion version, LanesLayoutStyle asymLaneType)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    foreach (var segment in info.m_segments)
                    {
                        if (asymLaneType != LanesLayoutStyle.Symmetrical)
                        {
                            var inverted = string.Empty;
                            if ((asymLaneType == LanesLayoutStyle.AsymL1R3 && ((segment.m_backwardForbidden & NetSegment.Flags.Invert) == 0))
                                || (asymLaneType == LanesLayoutStyle.AsymL3R1 && ((segment.m_backwardForbidden & NetSegment.Flags.Invert) != 0)))
                            {
                                inverted = "_Inverted";
                            }

                            segment.SetTextures(
                                new TextureSet
                                    (string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_Segment{0}__MainTex.png", inverted),
                                     string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_Segment{0}__AlphaMap.png", inverted)),
                                new LODTextureSet
                                    (string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_SegmentLOD{0}__MainTex.png", inverted),
                                     string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_SegmentLOD{0}__AlphaMap.png", inverted),
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_SegmentLOD__XYS.png"));
                        }
                    }
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    foreach (var segment in info.m_segments)
                    {
                        if (segment.m_mesh.name == "Elevated" || segment.m_mesh.name == "Bridge")
                        {
                            var inverted = (asymLaneType == LanesLayoutStyle.AsymL1R3 ? "_Inverted" : string.Empty);
                            segment.SetTextures(
                                new TextureSet
                                    (string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated{0}__MainTex.png", inverted),
                                     string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated_Segment{0}__APRMap.png", inverted)),
                                new LODTextureSet
                                    (string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated_LOD{0}__MainTex.png", inverted),
                                     string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated_SegmentLOD{0}__APRMap.png", inverted),
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated_LOD__XYSMap.png"));
                        }
                        else
                        {
                            segment.SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated__MainTex.png",
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated_Segment__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated_LOD__MainTex.png",
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated_SegmentLOD__APRMap.png",
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated_LOD__XYSMap.png"));
                        }
                    }

                    info.SetAllNodesTexture(
                        new TextureSet
                            (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated__MainTex.png",
                             @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated_Node__APRMap.png"),
                        new LODTextureSet
                            (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated_LOD__MainTex.png",
                             @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated_NodeLOD__APRMap.png",
                             @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Elevated_LOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    foreach (var segment in info.m_segments)
                    {
                        if (segment.m_mesh.name == "Slope")
                        {
                            var inverted = (asymLaneType == LanesLayoutStyle.AsymL1R3 ? "_Inverted" : string.Empty);
                            segment.SetTextures(
                                new TextureSet(
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_Segment{0}__MainTex.png", inverted),
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_Segment{0}__AlphaMap.png", inverted)),
                                new LODTextureSet(
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_SegmentLOD{0}__MainTex.png", inverted),
                                    string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_SegmentLOD{0}__AlphaMap.png", inverted),
                                    @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Slope_SegmentLOD2__XYSMap.png"));
                        }
                        else
                        {
                            segment.SetTextures(
                                new TextureSet
                                    (@"Roads\Highways\Highway4L\Textures\Slope_Segment__MainTex.png",
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Slope_Segment__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Slope_SegmentLOD__MainTex.png",
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Slope_SegmentLOD__APRMap.png",
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Slope_SegmentLOD__XYSMap.png"));
                        }
                    }

                    foreach (var node in info.m_nodes)
                    {
                        if (node.m_mesh.name == "Slope_Node")
                        {
                            node.SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_Segment__MainTex.png",
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_Node__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_SegmentLOD__MainTex.png",
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_SegmentLOD__AlphaMap.png",
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Ground_SegmentLOD__XYS.png"));
                        }
                        else
                        {
                            node.SetTextures(
                                new TextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_Segment__MainTex.png",
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_Node__APRMap.png"),
                                new LODTextureSet
                                    (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_NodeLOD__MainTex.png",
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_LOD__APRMap.png",
                                     @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_LOD__XYSMap.png"));
                        }
                    }
                    //info.SetAllNodesTexture(
                    //new TextureSet
                    //    (@"Roads\Highways\Highway4L\Textures\Slope_Node__MainTex.png",
                    //    @"Roads\Highways\Highway4L\Textures\Ground_Node__APRMap.png"),
                    //new LODTextureSet
                    //    (@"Roads\Highways\Highway4L\Textures\Ground_NodeLOD__MainTex.png",
                    //    @"Roads\Highways\Highway4L\Textures\Ground_NodeLOD__APRMap.png",
                    //    @"Roads\Highways\Highway4L\Textures\Ground_LOD__XYSMap.png"));
                    break;
                case NetInfoVersion.Tunnel:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            if (segment.m_mesh.name == "Tunnel")
                            {
                                var inverted = (asymLaneType == LanesLayoutStyle.AsymL1R3 ? "_Inverted" : string.Empty);
                                segment.SetTextures(
                                    new TextureSet
                                        (string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_Segment{0}__MainTex.png", inverted),
                                         string.Format(@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_Segment{0}__APRMap.png", inverted)),
                                    new LODTextureSet
                                        (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_NodeLOD__MainTex.png",
                                         @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_LOD__APRMap.png",
                                         @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_LOD__XYSMap.png"));
                            }
                            else
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_Segment__MainTex.png",
                                         @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_Segment__APRMap.png"),
                                    new LODTextureSet
                                        (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_NodeLOD__MainTex.png",
                                         @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_LOD__APRMap.png",
                                         @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_LOD__XYSMap.png"));
                            }
                        }

                        info.SetAllNodesTexture(
                            new TextureSet
                                (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_Segment__MainTex.png",
                                 @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_Node__APRMap.png"),
                            new LODTextureSet
                                (@"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_NodeLOD__MainTex.png",
                                 @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_LOD__APRMap.png",
                                 @"Roads\SmallHeavyRoads\AsymRoadL1R3\Textures\Tunnel_LOD__XYSMap.png"));
                        break;
                    }
            }
        }
    }
}

