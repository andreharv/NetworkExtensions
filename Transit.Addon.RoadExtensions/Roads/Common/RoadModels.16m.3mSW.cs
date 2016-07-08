using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup16m3mSWMesh(this NetInfo info, NetInfoVersion version, LanesLayoutStyle lanesLayoutStyle = LanesLayoutStyle.Symmetrical)
        {
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var highwaySlopeInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_SLOPE);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = info.m_segments[1];
                        var segments2 = info.m_segments[2];
                        var segments3 = info.m_segments[1].ShallowClone();

                        segments3.SetMeshes(@"Roads\Common\Meshes\16m\3mSW\BusStopInv.obj");
                        if (lanesLayoutStyle != LanesLayoutStyle.Symmetrical)
                        {
                            RoadHelper.HandleAsymComplementarySegmentsFlags(segments1, segments3, lanesLayoutStyle);

                            segments0.HandleAsymSegmentFlags(lanesLayoutStyle);
                            segments2.HandleAsymSegmentFlags(lanesLayoutStyle);
                        }

                        info.m_segments = new[] { segments0, segments1, segments2, segments3 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0];
                        var nodes0 = info.m_nodes[0];

                        segments0
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\3mSW\Elevated.obj",
                                @"Roads\Common\Meshes\16m\3mSW\Elevated_LOD.obj");

                        nodes0.SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW\Elevated_Node.obj",
                            @"Roads\Common\Meshes\16m\3mSW\Elevated_Node_LOD.obj");

                        segments0.HandleAsymSegmentFlags(lanesLayoutStyle);
                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0 };
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = highwaySlopeInfo.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[1];

                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node2 = node0.ShallowClone();

                        segment2
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW\Slope.obj",
                            @"Roads\Common\Meshes\16m\3mSW\Slope_LOD.obj");

                        node1
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW\Slope_Node.obj",
                            @"Roads\Common\Meshes\16m\3mSW\Slope_Node_LOD.obj");

                        node2
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW\Slope_U_Node.obj",
                            @"Roads\Common\Meshes\16m\3mSW\Slope_U_Node_LOD.obj");

                        segment2.HandleAsymSegmentFlags(lanesLayoutStyle);
                        node2.m_material = defaultMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2 };
                        info.m_nodes = new[] { node0, node1, node2 };

                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = segments0.ShallowClone();

                        var nodes0 = info.m_nodes[0];
                        var nodes1 = nodes0.ShallowClone();

                        segments1
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW\Tunnel.obj",
                            @"Roads\Common\Meshes\16m\3mSW\Tunnel_LOD.obj");

                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\3mSW\Tunnel_Node.obj",
                            @"Roads\Common\Meshes\16m\3mSW\Tunnel_Node_LOD.obj");

                        segments1.HandleAsymSegmentFlags(lanesLayoutStyle);
                        segments1.m_material = defaultMaterial;
                        nodes1.m_material = defaultMaterial;

                        info.m_segments = new[] { segments0, segments1 };
                        info.m_nodes = new[] { nodes0, nodes1 };

                        break;
                    }
            }
            return info;
        }
    }
}