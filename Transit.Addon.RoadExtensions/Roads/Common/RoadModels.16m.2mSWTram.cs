using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup16m2mSWMeshTram(this NetInfo info, NetInfoVersion version, LanesLayoutStyle laneStyle = LanesLayoutStyle.Symmetrical)
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
                        var segment4 = info.m_segments[1].ShallowClone();
                        if (laneStyle != LanesLayoutStyle.Symmetrical)
                            RoadHelper.HandleAsymSegmentFlags(segment1, segment3);
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[0].ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();
                        var node3 = info.m_nodes[0].ShallowClone();
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
                        segment4
                           .SetMeshes
                               (@"Roads\Common\Meshes\16m\2mSW\Ground_Tram.obj",
                               @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        segment4.m_backwardForbidden = NetSegment.Flags.Invert;
                        segment4.m_backwardRequired = NetSegment.Flags.None;

                        segment4.m_disableBendNodes = true;
                        segment4.m_emptyTransparent = true;
                        segment4.m_forwardForbidden = NetSegment.Flags.None;
                        segment4.m_forwardRequired = NetSegment.Flags.Invert;

                        node0
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Ground_Node.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Ground_Node_LOD.obj");
                        node1
                       .SetMeshes
                           (@"Roads\Common\Meshes\16m\2mSW\Ground_Node_Tram.obj",
                           @"Roads\Common\Meshes\16m\2mSW\Ground_Node_LOD.obj");
                        node1.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.SingleTram | NetInfo.ConnectGroup.Oneway | NetInfo.ConnectGroup.SingleMonorail;
                        node1.m_directConnect = true;
                        node1.m_emptyTransparent = true;

                        node2
                      .SetMeshes
                          (@"Roads\Common\Meshes\16m\2mSW\Ground_Node_Tram.obj",
                          @"Roads\Common\Meshes\16m\2mSW\Ground_Node_LOD.obj");
                        node2.m_connectGroup = NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.OnewayEnd | NetInfo.ConnectGroup.DoubleMonorail |NetInfo.ConnectGroup.MonorailStation;
                        node2.m_directConnect = true;
                        node2.m_emptyTransparent = true;


                        node3
                      .SetMeshes
                          (@"Roads\Common\Meshes\16m\2mSW\Ground_Node_Tram.obj",
                          @"Roads\Common\Meshes\16m\2mSW\Ground_Node_LOD.obj");
                        node3.m_connectGroup = NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.OnewayStart | NetInfo.ConnectGroup.DoubleMonorail;
                        node3.m_directConnect = true;
                        node3.m_emptyTransparent = true;


                        RoadHelper.HandleAsymSegmentFlags(segment0);
                        RoadHelper.HandleAsymSegmentFlags(segment2);
                        info.m_connectGroup = NetInfo.ConnectGroup.SingleTram;
                        info.m_dlcRequired = SteamHelper.DLC_BitMask.SnowFallDLC;
                        info.m_nodeConnectGroups = NetInfo.ConnectGroup.SingleTram | NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.SingleMonorail | NetInfo.ConnectGroup.DoubleMonorail;
                        info.m_requireDirectRenderers = true;
                        info.m_segments = new[] { segment0, segment1, segment2, segment3, segment4 };
                        info.m_nodes = new[] { node0, node1, node2, node3 };
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

                        RoadHelper.HandleAsymSegmentFlags(segment0);
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

                        RoadHelper.HandleAsymSegmentFlags(segment2);
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

                        RoadHelper.HandleAsymSegmentFlags(segment1);
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