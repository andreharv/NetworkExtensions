using Transit.Framework;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.TinyRoads.Plain2L
{
    public partial class PlainRoadBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (null,
                            @"Roads\Common\Textures\Plain\Ground_Segment__AlphaMap.png"),
                        new LODTextureSet
                           (@"Roads\Common\Textures\Plain\Ground_Segment_LOD__MainTex.png",
                            @"Roads\Common\Textures\Plain\Ground_Segment_LOD__AlphaMap.png",
                            @"Roads\Common\Textures\Plain\Ground_Segment_LOD__XYSMap.png"));

                    foreach (var node in info.m_nodes)
                    {
                        if (node.m_flagsRequired == NetNode.Flags.Transition)
                        {
                            node.SetTextures(
                                new TextureSet
                                   (null,
                                    @"Roads\Common\Textures\Plain\Ground_Trans__AlphaMap.png"),
                                new LODTextureSet
                                   (@"Roads\Common\Textures\Plain\Ground_Trans_LOD__MainTex.png",
                                    @"Roads\Common\Textures\Plain\Ground_Trans_LOD__AlphaMap.png",
                                    @"Roads\Common\Textures\Plain\Ground_Trans_LOD__XYSMap.png"));
                        }
                        else
                        {
                            node.SetTextures(
                                new TextureSet
                                   (null,
                                    @"Roads\Common\Textures\Plain\Ground_Segment__AlphaMap.png"),
                                new LODTextureSet
                                   (@"Roads\Common\Textures\Plain\Ground_Node_LOD__MainTex.png",
                                    @"Roads\Common\Textures\Plain\Ground_Node_LOD__AlphaMap.png",
                                    @"Roads\Common\Textures\Plain\Ground_Node_LOD__XYSMap.png"));
                        }
                    }
                    break;
            }
        }
    }
}
