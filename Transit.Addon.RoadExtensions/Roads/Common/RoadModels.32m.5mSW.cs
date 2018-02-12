using System.Linq;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup32m5mSWMesh(this NetInfo info, NetInfoVersion version)
        {
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments2 = info.m_segments[2].ShallowClone();
                        var segments3 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[0].ShallowClone();
                        segments0
                        .SetMeshes
                            (@"Roads\Common\Meshes\32m\5mSW\Ground.obj");
                        segments3.SetMeshes(
                                @"Roads\Common\Meshes\32m\5mSW\Ground_Parking.obj");
                        nodes0
                            .SetMeshes
                                 (@"Roads\Common\Meshes\32m\5mSW\Ground_Node_Parking.obj");
                        nodes1.SetMeshes(
                                @"Roads\Common\Meshes\32m\5mSW\Ground_Parking.obj");
                        info.m_segments = new[] { segments0, segments1, segments2, segments3 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                        break;
                    }
                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:
                case NetInfoVersion.GroundPavement:
                    {

                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments2 = info.m_segments[2].ShallowClone();
                        var segments3 = info.m_segments[0].ShallowClone();
                        var segments4 = info.m_segments[1].ShallowClone();
                        segments1.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Bus_Pavement.obj");
                        segments2.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\BusBoth_Pavement.obj");
                        segments3.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Ground_Pavement.obj");
                        segments4.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Ground_Pavement_Bus.obj");
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        nodes0.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Ground_Node_Pavement.obj");
                        var nodes1 = info.m_nodes[1].ShallowClone();
                        nodes1.SetMeshes(
                            @"Roads\Common\Meshes\32m\5mSW\Ground_Pavement.obj");
                        var segmentList = info.m_segments.ToList();
                        var nodeList = info.m_nodes.ToList();
                        segmentList[1] = segments1;
                        segmentList[2] = segments2;
                        segmentList[3] = segments3;
                        segmentList.Add(segments4);
                        nodeList[0] = nodes0;
                        nodeList[1] = nodes1;
                        info.m_segments = segmentList.ToArray();
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();

                        var node0 = info.m_nodes[0].ShallowClone();

                        info.m_segments = new[] { segment0, segment1 };
                        info.m_nodes = new[] { node0 };

                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();

                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node2 = node0.ShallowClone();

                        node2
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\5mSW\Slope_U_Node.obj",
                             @"Roads\Common\Meshes\32m\5mSW\Slope_U_Node.obj");

                        node2.m_material = defaultMaterial;

                        info.m_segments = new[] { segment0, segment1 };
                        info.m_nodes = new[] { node0, node1, node2 };

                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = segment0.ShallowClone();

                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = node0.ShallowClone();

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\5mSW\Tunnel.obj",
                             @"Roads\Highways\Common\Meshes\32m\Tunnel_LOD.obj");

                        node1
                            .SetFlags(NetNode.Flags.None,NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Common\Meshes\32m\5mSW\Tunnel_Node.obj",
                             @"Roads\Highways\Common\Meshes\32m\Tunnel_LOD.obj");

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