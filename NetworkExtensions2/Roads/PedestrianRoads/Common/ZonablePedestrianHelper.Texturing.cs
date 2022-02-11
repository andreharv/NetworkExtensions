using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Common
{
    public static partial class ZonablePedestrianHelper
    {
        public static void SetupGroundNakedTextures(this NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TextureSet(
                            @"Roads\Common\Textures\Plain\Ground_Segment__AlphaMap.png",
                            @"Roads\Common\Textures\Plain\Ground_Segment__AlphaMap.png"),
                        new LODTextureSet(
                            @"Roads\Common\Textures\Plain\Ground_Segment_LOD__AlphaMap.png",
                            @"Roads\Common\Textures\Plain\Ground_Segment_LOD__AlphaMap.png",
                            @"Roads\Common\Textures\Plain\Ground_Node_LOD__XYSMap.png"));

                    foreach (var node in info.m_nodes)
                    {
                        if (node.m_flagsRequired != NetNode.Flags.Transition)
                        {
                            node.SetTextures(
                                new TextureSet
                                    (@"Roads\Common\Textures\Plain\Ground_Segment__AlphaMap.png",
                                     @"Roads\Common\Textures\Plain\Ground_Segment__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__MainTex.png",
                                     @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__AlphaMap.png",
                                     @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__XYSMap.png"));
                        }
                        else
                        {
                            node.SetTextures(
                                new TextureSet
                                    (@"Roads\PedestrianRoads\Common\Textures\Ground_Trans__MainTex.png",
                                     @"Roads\PedestrianRoads\Common\Textures\Ground_Trans__AlphaMap.png",
                                     @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__XYSMap.png"),
                            new LODTextureSet
                                    (@"Roads\PedestrianRoads\Common\Textures\Ground_Trans_LOD__MainTex.png",
                                     @"Roads\PedestrianRoads\Common\Textures\Ground_Trans_LOD__AlphaMap.png",
                                     @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__XYSMap.png"));
                        }
                    }
                    break;
            }
        }

        public static void SetupElevatedPavedTextures(this NetInfo info, NetInfoVersion version)
        {
            info.SetAllSegmentsTexture(
                new TextureSet
                   (@"Roads\Common\Textures\Plain\Elevated_Segment__MainTex.png",
                    @"Roads\Common\Textures\Plain\Elevated_Segment__AlphaMap.png",
                    @"Roads\PedestrianRoads\Common\Textures\Elevated_Segment__XYSMap.png"),
                new LODTextureSet
                   (@"Roads\Common\Textures\Plain\Elevated_Segment_LOD__MainTex.png",
                    @"Roads\Common\Textures\Plain\Elevated_Segment_LOD__AlphaMap.png",
                    @"Roads\Common\Textures\Plain\Elevated_Segment_LOD__XYSMap.png"));

            foreach (var node in info.m_nodes)
            {
                if (node.m_flagsRequired != NetNode.Flags.Transition)
                {
                    node.SetTextures(
                        new TextureSet
                            (@"Roads\PedestrianRoads\Common\Textures\Elevated_Node__MainTex.png",
                            @"Roads\Common\Textures\Plain\Elevated_Segment__AlphaMap.png"),
                        new LODTextureSet
                            (@"Roads\Common\Textures\Plain\Elevated_Segment_LOD__MainTex.png",
                            @"Roads\Common\Textures\Plain\Elevated_Segment_LOD__AlphaMap.png",
                            @"Roads\Common\Textures\Plain\Elevated_Segment_LOD__XYSMap.png"));
                }
                else
                {
                    node.SetTextures(
                        new TextureSet
                            (@"Roads\PedestrianRoads\Common\Textures\Elevated_Trans__MainTex.png",
                             @"Roads\Common\Textures\Plain\Elevated_Trans__AlphaMap.png"),
                        new LODTextureSet
                            (@"Roads\PedestrianRoads\Common\Textures\Elevated_Trans_LOD__MainTex.png",
                            @"Roads\PedestrianRoads\Common\Textures\Elevated_Trans_LOD__AlphaMap.png",
                            @"Roads\Common\Textures\Plain\Elevated_Segment_LOD__XYSMap.png")
                        );
                }
            }
        }

        public static void SetupBoardWalkTextures(this NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                case NetInfoVersion.Elevated:
                    info.SetAllSegmentsTexture(
                        new TextureSet(
                            @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment__MainTex.png",
                            @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment__AlphaMap.png",
                            @"Roads\PedestrianRoads\Common\Textures\Ground_Node__XYSMap.png"),
                        new LODTextureSet(
                            @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment_LOD__MainTex.png",
                            @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment_LOD__AlphaMap.png",
                            @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__XYSMap.png")
                        );
                    var groundElevated = "Ground";
                    if (version == NetInfoVersion.Elevated)
                        groundElevated = "Elevated";
                    foreach (var node in info.m_nodes)
                    {
                        if (node.m_mesh.name.ToLower().Contains("nocom"))
                        {
                            node.SetTextures(
                                new TextureSet(
                                        @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment__MainTex.png",
                                        @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment__AlphaMap.png",
                                        @"Roads\PedestrianRoads\Common\Textures\Ground_Node__XYSMap.png"),
                                    new LODTextureSet(
                                        @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment_LOD__MainTex.png",
                                        @"Roads\PedestrianRoads\BoardWalkTiny\Textures\Elevated_Segment_LOD__AlphaMap.png",
                                        @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__XYSMap.png")
                                    );
                        }
                        else if (node.m_flagsRequired == NetNode.Flags.Transition)
                        {
                            node.SetTextures(
                                new TextureSet
                                    ($@"Roads\PedestrianRoads\BoardwalkTiny\Textures\{groundElevated}_Trans__MainTex.png",
                                    $@"Roads\PedestrianRoads\BoardwalkTiny\Textures\{groundElevated}_Trans__AlphaMap.png"),
                                new LODTextureSet
                                    ($@"Roads\PedestrianRoads\Common\Textures\{groundElevated}_Trans_LOD__MainTex.png",
                                    $@"Roads\PedestrianRoads\Common\Textures\{groundElevated}_Trans_LOD__AlphaMap.png",
                                    @"Roads\Common\Textures\Plain\Elevated_Segment_LOD__XYSMap.png")
                                );
                        }
                        else
                        {
                            node.SetTextures(
                                new TextureSet
                                    (@"Roads\Common\Textures\Plain\Ground_Segment__AlphaMap.png",
                                     @"Roads\Common\Textures\Plain\Ground_Segment__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__MainTex.png",
                                     @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__AlphaMap.png",
                                     @"Roads\PedestrianRoads\Common\Textures\Ground_Node_LOD__XYSMap.png"));
                        }
                    }
                    break;
            }
        }
    }
}
