using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup32m5mSW3mMdn(this NetInfo info, NetInfoVersion version, AsymLaneType asymLaneType = AsymLaneType.L0R0)
        {
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = info.m_segments[1];
                        var segments2 = info.m_segments[2];
                        var segments3 = info.m_segments[1].ShallowClone();
                        segments0.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSw3mMdn\Ground.obj");
                        segments1.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSw3mMdn\BusInv.obj");
                        segments2.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSw3mMdn\BusBoth.obj");
                        segments3.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSw3mMdn\Bus.obj");
                        if (asymLaneType != AsymLaneType.L0R0)
                            RoadHelper.HandleAsymComplementarySegmentsFlags(segments1, segments3, asymLaneType);

                        segments0.HandleAsymSegmentFlags(asymLaneType);
                        segments2.HandleAsymSegmentFlags(asymLaneType);
                        info.m_segments = new[] { segments0, segments1, segments2, segments3 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0];
                        //var nodes0 = info.m_nodes[0];

                        segments0
                            .SetMeshes
                                (@"Roads\Common\Meshes\32m\5mSw3mMdn\Elevated.obj");

                        //nodes0.SetMeshes
                        //    (@"Roads\Common\Meshes\32m\5mSw3mMdn\Elevated_Node.obj");

                        segments0.HandleAsymSegmentFlags(asymLaneType);
                        info.m_segments = new[] { segments0 };
                        //info.m_nodes = new[] { nodes0 };
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[1];

                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node2 = node0.ShallowClone();

                        segment2
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\5mSw3mMdn\Slope.obj");

                        //node1
                        //    .SetMeshes
                        //    (@"Roads\Common\Meshes\32m\5mSw3mMdn\Slope_Node.obj");

                        node2
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\5mSw3mMdn\Slope_U_Node.obj",
                             @"Roads\Common\Meshes\32m\5mSW\Slope_U_Node.obj");

                        segment2.HandleAsymSegmentFlags(asymLaneType);
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
                            (@"Roads\Common\Meshes\32m\5mSw3mMdn\Tunnel.obj");

                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\5mSw3mMdn\Tunnel_Node.obj",
                             @"Roads\Highways\Common\Meshes\32m\Tunnel_LOD.obj");

                        segments1.HandleAsymSegmentFlags(asymLaneType);
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