using System.Collections.Generic;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup16m3mSW3mMdnMesh(this NetInfo info, NetInfoVersion version, LanesLayoutStyle lanesLayoutStyle = LanesLayoutStyle.Symmetrical)
        {
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var highwaySlopeInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_SLOPE);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            switch (version)
            {
                case NetInfoVersion.Ground:
                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = info.m_segments[1];
                        var segments2 = info.m_segments[2];
                        var segments3 = info.m_segments[0].ShallowClone();

                        segments3.SetMeshes(@"Roads\Common\Meshes\16m\3mSW3mMdn\Ground_Segment_Median.obj", @"Roads\Common\Meshes\16m\3mSW3mMdn\Ground_Segment_Median_LOD.obj");

                        var theNodes = new List<NetInfo.Node>();
                        theNodes.AddRange(info.m_nodes);
                        var medianNode = info.m_nodes[0].ShallowClone();
                        medianNode.SetMeshes(@"Roads\Common\Meshes\16m\3mSW3mMdn\Ground_Node_Median.obj", @"Roads\Common\Meshes\blank.obj");
                        theNodes.Add(medianNode);
                        info.m_segments = new[] { segments0, segments1, segments2, segments3 };
                        info.m_nodes = theNodes.ToArray();
                        break;
                    }
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0];
                        var nodes1 = info.m_nodes[0].ShallowClone();
                        
                        segments0
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\3mSW\Elevated.obj",
                                @"Roads\Common\Meshes\16m\3mSW\Elevated_LOD.obj");
                        segments1
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\3mSW3mMdn\Elevated_Segment_Median.obj",
                                @"Roads\Common\Meshes\16m\3mSW3mMdn\Ground_Segment_Median_LOD.obj");
                        nodes0
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW\Elevated_Node.obj",
                            @"Roads\Common\Meshes\16m\3mSW\Elevated_Node_LOD.obj");
                        nodes1
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW3mMdn\Elevated_Node_Median.obj",
                            @"Roads\Common\Meshes\blank.obj")
                            .SetConsistentUVs();

                        info.m_segments = new[] { segments0, segments1 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = highwaySlopeInfo.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[1];
                        var segment3 = info.m_segments[1].ShallowClone();
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node2 = node0.ShallowClone();
                        var node3 = node1.ShallowClone();
                        var node4 = node0.ShallowClone();
                        segment2
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW\Slope.obj",
                            @"Roads\Common\Meshes\16m\3mSW\Slope_LOD.obj");
                        segment3
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\3mSW3mMdn\Elevated_Segment_Median.obj",
                                @"Roads\Common\Meshes\16m\3mSW3mMdn\Ground_Segment_Median_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW\Slope_Node.obj",
                            @"Roads\Common\Meshes\16m\3mSW\Slope_Node_LOD.obj");

                        node2
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW\Slope_U_Node.obj",
                            @"Roads\Common\Meshes\16m\3mSW\Slope_U_Node_LOD.obj");

                        node3
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW3mMdn\Elevated_Node_Median.obj",
                            @"Roads\Common\Meshes\blank.obj");

                        node4
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW3mMdn\Elevated_Node_Median.obj",
                            @"Roads\Common\Meshes\blank.obj");


                        node2.m_material = defaultMaterial;
                        node4.m_material = defaultMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = new[] { node0, node1, node2, node3, node4 };

                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = segments0.ShallowClone();
                        var segments2 = segments0.ShallowClone();
                        var nodes0 = info.m_nodes[0];
                        var nodes1 = nodes0.ShallowClone();
                        var nodes2 = nodes0.ShallowClone();
                        segments1
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW\Tunnel.obj",
                            @"Roads\Common\Meshes\16m\3mSW\Tunnel_LOD.obj");
                        segments2
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\3mSW3mMdn\Elevated_Segment_Median.obj",
                                @"Roads\Common\Meshes\16m\3mSW3mMdn\Ground_Segment_Median_LOD.obj");
                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW\Tunnel_Node.obj",
                            @"Roads\Common\Meshes\16m\3mSW\Tunnel_Node_LOD.obj");
                        nodes2
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW3mMdn\Elevated_Node_Median.obj",
                            @"Roads\Common\Meshes\blank.obj");

                        segments1.m_material = defaultMaterial;
                        segments2.m_material = defaultMaterial;
                        nodes1.m_material = defaultMaterial;
                        nodes2.m_material = defaultMaterial;
                        info.m_segments = new[] { segments0, segments1, segments2 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2 };

                        break;
                    }
            }
            return info;
        }
    }
}