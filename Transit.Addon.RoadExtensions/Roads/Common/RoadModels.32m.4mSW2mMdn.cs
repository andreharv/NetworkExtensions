using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup32m4mSW2mMdn(this NetInfo info, NetInfoVersion version, LanesLayoutStyle layoutStyle = LanesLayoutStyle.Symmetrical)
        {
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;
            //var inverted = string.Empty;
            //if (L > R)
            //{
            //    inverted = "_Inverted";
            //}
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = info.m_segments[1];
                        var segments2 = info.m_segments[2];
                        var segments3 = info.m_segments[1].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        segments0.SetMeshes(
                            @"Roads\Common\Meshes\32m\4mSw2mMdn\Ground.obj");
                        segments1.SetMeshes(
                            @"Roads\Common\Meshes\32m\4mSw2mMdn\Bus.obj");
                        segments2.SetMeshes(
                            @"Roads\Common\Meshes\32m\4mSw2mMdn\BusBoth.obj");
                        segments3.SetMeshes(
                            @"Roads\Common\Meshes\32m\4mSw2mMdn\BusInv.obj");
                        nodes0.SetMeshes(
                            @"Roads\Common\Meshes\32m\4mSW2mMdn\Ground_Node.obj");
                        if (layoutStyle != LanesLayoutStyle.Symmetrical)
                        {
                            
                            RoadHelper.HandleAsymSegmentFlags(segments3, segments1);
                            RoadHelper.HandleAsymSegmentFlags(segments0);
                        }

                        info.m_segments = new[] { segments0, segments1, segments2, segments3 };
                        info.m_nodes = new[] { nodes0 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segments0 = info.m_segments[0];
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        segments0
                            .SetMeshes
                                (@"Roads\Common\Meshes\32m\4mSw2mMdn\Elevated.obj");
                        nodes0
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\4mSW2mMdn\Elevated_Node.obj");
                        if (layoutStyle != LanesLayoutStyle.Symmetrical)
                            RoadHelper.HandleAsymSegmentFlags(segments0);

                        info.m_segments = new[] { segments0};
                        info.m_nodes = new[] { nodes0 };
                        //info.m_nodes = new[] { nodes0 };
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = info.m_segments[1];
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        segments0
                            .SetMeshes
                                (@"Roads\Common\Meshes\32m\4mSw2mMdn\Elevated.obj");
                        nodes0
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\4mSW2mMdn\Elevated_Node.obj");
                        if (layoutStyle != LanesLayoutStyle.Symmetrical)
                            RoadHelper.HandleAsymSegmentFlags(segments0);

                        info.m_segments = new[] { segments0,segments1 };
                        info.m_nodes = new[] { nodes0 };
                        //info.m_nodes = new[] { nodes0 };
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[1].ShallowClone();
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node2 = node0.ShallowClone();

                        segment1
                            .SetMeshes
                            ($@"Roads\Common\Meshes\32m\4mSw2mMdn\Slope.obj");
                        node1
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\4mSW2mMdn\Slope_Node.obj");
                        node2
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\4mSw2mMdn\Slope_U_Node.obj",
                             @"Roads\Common\Meshes\32m\5mSW\Slope_U_Node.obj");

                        RoadHelper.HandleAsymSegmentFlags(segment1);
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
                            ($@"Roads\Common\Meshes\32m\4mSw2mMdn\Tunnel.obj");
                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\4mSw2mMdn\Tunnel_Node.obj",
                             @"Roads\Highways\Common\Meshes\32m\Tunnel_LOD.obj");

                        if (layoutStyle != LanesLayoutStyle.Symmetrical)
                            RoadHelper.HandleAsymSegmentFlags(segments1);
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