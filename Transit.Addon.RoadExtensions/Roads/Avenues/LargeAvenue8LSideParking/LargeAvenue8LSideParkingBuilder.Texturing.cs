using Transit.Framework;
using Transit.Framework.Texturing;
#if DEBUG
using Debug = Transit.Framework.Debug;
#endif
namespace TransitPlus.Addon.RoadExtensions.Roads.Avenues.LargeAvenue8LSideParking
{
    public partial class LargeAvenue8LSideParkingBuilder
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
                            (@"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Segment__MainTex.png",
                            $@"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Segment{suffix}__APRMap.png"),
                    new LODTextureSet
                            (@"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Segment_LOD__MainTex.png",
                             $@"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Segment{suffix}_LOD__APRMap.png",
                            @"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Segment_LOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TextureSet
                            (@"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Node__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Node__APRMap.png"),
                        new LODTextureSet
                           (@"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Node_LOD__MainTex.png",
                            @"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Node_LOD__APRMap.png",
                            @"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Segment_LOD__XYSMap.png"));


                    foreach (var node in info.m_nodes)
                    {
                        if (node.m_flagsForbidden == NetNode.Flags.Transition)
                        {
                            info.m_nodes[0].SetTextures(
                                  new TextureSet
                                      (@"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Node_Simple__MainTex.png",
                                      @"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Node_Simple__APRMap.png"),
                                  new LODTextureSet
                                     (@"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Node_LOD__MainTex.png",
                                      @"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Node_LOD__APRMap.png",
                                      @"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Segment_LOD__XYSMap.png"));
                        }
                        else if (node.m_flagsRequired == NetNode.Flags.Transition)
                        {
                            node.SetTextures(
                                    new TextureSet
                                        (@"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Node__MainTex.png",
                                        @"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Node__APRMap.png"),
                                    new LODTextureSet
                                       (@"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Node_LOD__MainTex.png",
                                        @"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Node_LOD__APRMap.png",
                                        @"Roads\Avenues\LargeAvenue8LSideParking\Textures\Ground_Segment_LOD__XYSMap.png"));
                        }
                        
                    }
                    break;
            }
        }
    }

}

