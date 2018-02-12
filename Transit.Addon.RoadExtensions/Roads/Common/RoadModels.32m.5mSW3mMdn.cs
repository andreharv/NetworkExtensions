using System.Linq;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup32m5mSW3mMdn(this NetInfo info, NetInfoVersion version, LanesLayoutStyle layoutStyle = LanesLayoutStyle.Symmetrical)
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
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments2 = info.m_segments[2].ShallowClone();
                        var segments3 = info.m_segments[1].ShallowClone();
                        var segments4 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[0].ShallowClone();
                        segments0.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSw3mMdn\Ground.obj");
                        segments1.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSw3mMdn\Bus.obj");
                        segments2.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSw3mMdn\BusBoth.obj");
                        segments3.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSw3mMdn\BusInv.obj");
                        segments4.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Ground_Parking.obj");
                        nodes0.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Ground_Node_Parking.obj");
                        nodes1.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Ground_Parking.obj");
                        if (layoutStyle != LanesLayoutStyle.Symmetrical)
                        {
                            RoadHelper.HandleAsymSegmentFlags(segments3, segments1);
                            RoadHelper.HandleAsymSegmentFlags(segments0);
                        }

                        info.m_segments = new[] { segments0, segments1, segments2, segments3, segments4};
                        info.m_nodes = new[] { nodes0, nodes1 };
                        break;
                    }
                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:
                case NetInfoVersion.GroundPavement:
                    {

                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments2 = info.m_segments[2].ShallowClone();
                        var segments3 = info.m_segments[3].ShallowClone();
                        var segments4 = info.m_segments[0].ShallowClone();
                        var segments5 = info.m_segments[1].ShallowClone();
                        var segments6 = info.m_segments[3].ShallowClone();
                        segments1.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSw3mMdn\Bus_Pavement.obj");
                        segments2.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSw3mMdn\BusBoth_Pavement.obj");
                        segments3.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSw3mMdn\BusInv_Pavement.obj");
                        segments4.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Ground_Pavement.obj");
                        segments5.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Ground_Pavement_Bus.obj");
                        segments6.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Ground_Pavement_Bus.obj");
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();
                        nodes0.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Ground_Node_Pavement.obj");
                        nodes1.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Ground_Pavement.obj");
                        var segmentList = info.m_segments.ToList();
                        var nodeList = info.m_nodes.ToList();
                        segmentList[1] = segments1;
                        segmentList[2] = segments2;
                        segmentList[3] = segments3;
                        segmentList[4] = segments4;
                        segmentList.Add(segments5);
                        segmentList.Add(segments6);
                        nodeList[0] = nodes0;
                        nodeList[1] = nodes1;
                        info.m_segments = segmentList.ToArray();
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();

                        segments0
                            .SetMeshes
                                ($@"Roads\Common\Meshes\32m\5mSw3mMdn\Elevated.obj");

                        if (layoutStyle != LanesLayoutStyle.Symmetrical)
                            RoadHelper.HandleAsymSegmentFlags(segments0);

                        info.m_segments = new[] { segments0 };
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();

                        segments0
                            .SetMeshes
                                ($@"Roads\Common\Meshes\32m\5mSw3mMdn\Elevated.obj");

                        if (layoutStyle != LanesLayoutStyle.Symmetrical)
                            RoadHelper.HandleAsymSegmentFlags(segments0);

                        info.m_segments = new[] { segments0, segments1 };
                        //info.m_nodes = new[] { nodes0 };
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[1].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node2 = node0.ShallowClone();

                        segment1
                            .SetMeshes
                            ($@"Roads\Common\Meshes\32m\5mSw3mMdn\Slope.obj",
                            @"Roads\Common\Meshes\32m\3mSw2mMdn\Slope_LOD.obj");
                        node2
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\5mSw3mMdn\Slope_U_Node.obj",
                             @"Roads\Common\Meshes\32m\5mSW\Slope_U_Node.obj");

                        RoadHelper.HandleAsymSegmentFlags(segment1);
                        node2.m_material = defaultMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2 };
                        info.m_nodes = new[] { node0, node1, node2 };

                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = segments0.ShallowClone();
                        var segments2 = segments0.ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = nodes0.ShallowClone();

                        segments1
                            .SetMeshes
                            ($@"Roads\Common\Meshes\32m\5mSw3mMdn\Tunnel.obj");
                        segments2
                            .SetMeshes
                            ($@"Roads\Common\Meshes\32m\5mSw3mMdn\Tunnel_Inverted.obj");
                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\5mSw3mMdn\Tunnel_Node.obj",
                             @"Roads\Highways\Common\Meshes\32m\Tunnel_LOD.obj");

                        if (layoutStyle != LanesLayoutStyle.Symmetrical)
                            RoadHelper.HandleAsymSegmentFlags(segments1, segments2);
                        segments1.m_material = defaultMaterial;
                        nodes1.m_material = defaultMaterial;

                        info.m_segments = new[] { segments0, segments1, segments2 };
                        info.m_nodes = new[] { nodes0, nodes1 };

                        break;
                    }
            }
            return info;
        }
    }
}