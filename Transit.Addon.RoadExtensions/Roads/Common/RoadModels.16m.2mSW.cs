using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup16m2mSWMesh(this NetInfo info, NetInfoVersion version, LanesLayoutStyle laneStyle = LanesLayoutStyle.Symetrical)
        {
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var highwaySlopeInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_SLOPE);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1];
                        var segment2 = info.m_segments[2];
                        var segment3 = info.m_segments[1].ShallowClone();

                        if (laneStyle != LanesLayoutStyle.Symetrical)
                            RoadHelper.HandleAsymComplementarySegmentsFlags(segment1, segment3, laneStyle);
                        var node0 = info.m_nodes[0];

                        segment0
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Ground.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        segment1
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Ground.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        segment2
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Ground.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        segment3
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Ground.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        node0
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Ground_Node.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Ground_Node_LOD.obj");

                        segment0.HandleAsymSegmentFlags(laneStyle);
                        segment2.HandleAsymSegmentFlags(laneStyle);
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = new[] { node0 };

                        break;
                    }
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        var segment0 = info.m_segments[0];
                        var node0 = info.m_nodes[0];

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Elevated.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Elevated_LOD.obj");

                        node0.
                            SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Elevated_Node.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Elevated_Node_LOD.obj");

                        segment0.HandleAsymSegmentFlags(laneStyle);
                        info.m_segments = new[] { segment0 };
                        info.m_nodes = new[] { node0 };
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
                        //segment0
                        //    .SetFlagsDefault()
                        //    **ToDo

                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\2mSW\Slope.obj",
                            @"Roads\Common\Meshes\16m\2mSW\Slope_LOD.obj");

                        node1
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\2mSW\Slope_Node.obj",
                            @"Roads\Common\Meshes\16m\2mSW\Slope_Node_LOD.obj");

                        node2
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\2mSW\Slope_U_Node.obj",
                            @"Roads\Common\Meshes\16m\2mSW\Slope_U_Node_LOD.obj");

                        segment2.HandleAsymSegmentFlags(laneStyle);
                        node2.m_material = defaultMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2 };
                        info.m_nodes = new[] { node0, node1, node2 };

                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = segment0.ShallowClone();

                        var node0 = info.m_nodes[0];
                        var node1 = node0.ShallowClone();

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\2mSW\Tunnel.obj",
                            @"Roads\Common\Meshes\16m\2mSW\Tunnel_LOD.obj");

                        node1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\16m\2mSW\Tunnel_Node.obj",
                            @"Roads\Common\Meshes\16m\2mSW\Tunnel_Node_LOD.obj");

                        segment1.HandleAsymSegmentFlags(laneStyle);
                        segment1.m_material = defaultMaterial;
                        node1.m_material = defaultMaterial;

                        info.m_segments = new[] { segment0, segment1 };
                        info.m_nodes = new[] { node0, node1 };

                        break;
                    }
            }
            return info;
        }
    }
}