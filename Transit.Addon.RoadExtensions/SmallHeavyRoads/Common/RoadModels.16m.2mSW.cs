using Transit.Framework;

namespace Transit.Addon.RoadExtensions.SmallHeavyRoads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup16m2mSWMesh(this NetInfo info, NetInfoVersion version)
        {
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var highwaySlopeInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_SLOPE);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = info.m_segments[0];
                        var node0 = info.m_nodes[0];

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                                (@"SmallHeavyRoads\Common\Meshes\16m\2mSW\Ground.obj",
                                @"SmallHeavyRoads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        node0
                            .SetMeshes
                                (@"SmallHeavyRoads\Common\Meshes\16m\2mSW\Ground_Node.obj",
                                @"SmallHeavyRoads\Common\Meshes\16m\2mSW\Ground_Node_LOD.obj");
                        info.m_segments = new[] { segment0 };
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
                                (@"SmallHeavyRoads\Common\Meshes\16m\2mSW\Elevated.obj",
                                @"SmallHeavyRoads\Common\Meshes\16m\2mSW\Elevated_LOD.obj");

                        node0.
                            SetMeshes
                                (@"SmallHeavyRoads\Common\Meshes\16m\2mSW\Elevated_Node.obj",
                                @"SmallHeavyRoads\Common\Meshes\16m\2mSW\Elevated_Node_LOD.obj");

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
                            (@"SmallHeavyRoads\Common\Meshes\16m\2mSW\Slope.obj",
                            @"SmallHeavyRoads\Common\Meshes\16m\2mSW\Slope_LOD.obj");

                        node1
                            .SetMeshes
                            (@"SmallHeavyRoads\Common\Meshes\16m\2mSW\Slope_Node.obj",
                            @"SmallHeavyRoads\Common\Meshes\16m\2mSW\Slope_Node_LOD.obj");

                        node2
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"SmallHeavyRoads\Common\Meshes\16m\2mSW\Slope_U_Node.obj",
                            @"SmallHeavyRoads\Common\Meshes\16m\2mSW\Slope_U_Node_LOD.obj");

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
                            (@"SmallHeavyRoads\Common\Meshes\16m\2mSW\Tunnel.obj",
                            @"SmallHeavyRoads\Common\Meshes\16m\2mSW\Tunnel_LOD.obj");

                        node1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"SmallHeavyRoads\Common\Meshes\16m\2mSW\Tunnel_Node.obj",
                            @"SmallHeavyRoads\Common\Meshes\16m\2mSW\Tunnel_Node_LOD.obj");

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