using System.Collections.Generic;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup32m3mSW2x3mMdnBusMesh(this NetInfo info, NetInfoVersion version)
        {
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            switch (version)
            {
                case NetInfoVersion.Ground:
                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:
                case NetInfoVersion.Elevated:
                    {
                        var segmentList = new List<NetInfo.Segment>();
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments2 = info.m_segments[2].ShallowClone();
                        var segments4 = info.m_segments[0].ShallowClone();
                        var coarseVersion = version == NetInfoVersion.Elevated ? "Elevated" : "Ground";
                        segments0
                            .SetMeshes
                            ($@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\{coarseVersion}.obj");
                        segments1
                            .SetMeshes
                                ($@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Bus.obj");
                        segments2
                            .SetMeshes
                                ($@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_BusBoth.obj");
                        segments4
                            .SetFlagsDefault()
                            .SetMeshes
                                (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Median.obj",
                                @"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Median_LOD.obj");
                        segmentList.Add(segments0);
                        segmentList.Add(segments1);
                        segmentList.Add(segments2);
                        segmentList.Add(segments4);
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        if (version == NetInfoVersion.Elevated)
                        {
                            nodes0.SetMeshes
                                ($@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Elevated_Node.obj");
                        }
                        else if (version == NetInfoVersion.Ground)
                        {
                            var segments3 = info.m_segments[0].ShallowClone();
                            segmentList.Add(segments3);

                            segments3
                                .SetMeshes
                                    ($@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Parking.obj");
                            nodes0.SetMeshes
                                    ($@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Node.obj");
                        }
                        else
                        {
                            var segments3 = info.m_segments[0].ShallowClone();
                            segmentList.Add(segments3);

                            segments3
                                .SetMeshes
                                    ($@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Pavement.obj");
                            nodes0.SetMeshes
                                    ($@"Roads\Common\Meshes\32m\4mSW2mMdn\{coarseVersion}_Node.obj");
                        }

                        var nodes1 = info.m_nodes[0].ShallowClone();
                        nodes1.SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Node_Median.obj");

                        info.m_segments = segmentList.ToArray();
                        info.m_nodes = new[] { nodes0, nodes1 };
                        break;
                    }

                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments2 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[0].ShallowClone();

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                                (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Elevated.obj",
                                 @"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Elevated_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                                (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Bridge_Cables.obj",
                                @"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Bridge_Cables_LOD.obj");
                        segments2
                            .SetFlagsDefault()
                            .SetMeshes
                                (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Median.obj",
                                @"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Median_LOD.obj");

                        nodes0
                            .SetMeshes
                                (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Elevated_Node.obj",
                                 @"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Elevated_Node_LOD.obj");
                        nodes1.SetMeshes
                                (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Node_Median.obj");

                        info.m_segments = new[] { segments0, segments1, segments2 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[1].ShallowClone();
                        var segment3 = info.m_segments[1].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node2 = node0.ShallowClone();
                        var node3 = info.m_nodes[1].ShallowClone();

                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Slope.obj",
                            @"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Slope_LOD.obj");
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                               (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Median.obj",
                                @"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Median_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Slope_Node.obj",
                            @"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Slope_Node_LOD.obj");

                        node2
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Slope_U_Node.obj",
                            @"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Slope_U_Node_LOD.obj");
                        node3.SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Node_Median.obj");

                        node2.m_material = defaultMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = new[] { node0, node1, node2, node3 };

                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = segment0.ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = node0.ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Tunnel.obj",
                            @"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Tunnel_LOD.obj");
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                                (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Median.obj",
                                @"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Median_LOD.obj");
                        node1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Tunnel_Node.obj",
                            @"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Tunnel_Node_LOD.obj");

                        node2.SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2x3mMdnBus\Ground_Node_Median.obj");

                        segment1.m_material = defaultMaterial;
                        segment2.m_material = defaultMaterial;
                        node1.m_material = defaultMaterial;
                        node2.m_material = defaultMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2 };
                        info.m_nodes = new[] { node0, node1, node2 };

                        break;
                    }
            }
            return info;
        }
    }
}