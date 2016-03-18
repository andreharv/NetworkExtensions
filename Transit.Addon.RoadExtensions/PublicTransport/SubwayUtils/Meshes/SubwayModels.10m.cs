using System.Collections.Generic;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.PublicTransport.SubwayUtils
{
    public static partial class SubwayModels
    {
        public static void Setup10mMesh(this NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var ttInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.TRAINTRACK);
            var defaultMaterial = ttInfo.m_nodes[0].m_material;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = info.m_segments[1];
                        var segments2 = info.m_segments[2];
                        var nodes0 = info.m_nodes[0];
                        var nodes1 = info.m_nodes[1];
                        var nodes2 = info.m_nodes[2];
                        var nodes3 = info.m_nodes[3];
                        var nodes4 = info.m_nodes[1].ShallowClone();
                        var nodes5 = info.m_nodes[1].ShallowClone();
                        var nodes6 = info.m_nodes[3].ShallowClone();
                        var nodes7 = info.m_nodes[3].ShallowClone();

                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes3.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes4.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes7.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"PublicTransport\SubwayUtils\Meshes\10m\Ground_Pavement.obj");

                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"PublicTransport\SubwayUtils\Meshes\10m\Ground_Rail.obj");

                        segments2
                            .SetMeshes
                            (@"PublicTransport\SubwayUtils\Meshes\10m\Ground_Power.obj");

                        nodes0
                            .SetMeshes
                            (@"PublicTransport\SubwayUtils\Meshes\10m\Ground_Pavement_Node.obj");

                        nodes1
                            .SetMeshes
                            (@"PublicTransport\SubwayUtils\Meshes\10m\Ground_Rail.obj");
                        nodes3
                            .SetMeshes
                            (@"PublicTransport\SubwayUtils\Meshes\10m\Ground_Power.obj");
                        nodes4
                            .SetMeshes
                            (@"PublicTransport\SubwayUtils\Meshes\10m\Ground_Rail_Start.obj");
                        nodes5
                            .SetMeshes
                            (@"PublicTransport\SubwayUtils\Meshes\10m\Ground_Rail_End.obj");
                        nodes6
                            .SetMeshes
                            (@"PublicTransport\SubwayUtils\Meshes\10m\Ground_Power_Start.obj");
                        nodes7
                            .SetMeshes
                            (@"PublicTransport\SubwayUtils\Meshes\10m\Ground_Power_End.obj");

                        var colors = new List<UnityEngine.Color>();
                        var colors32 = new List<UnityEngine.Color32>();
                        var tangents = new List<UnityEngine.Vector4>();

                        for (int i = 0; i < segments2.m_mesh.vertexCount; i++)
                        {
                            colors.Add(new UnityEngine.Color(0, 0, 0, 255));
                            colors32.Add(new UnityEngine.Color32(0, 0, 0, 255));
                            tangents.Add(new UnityEngine.Vector4(0, 0, 1, -1));
                        }

                        segments2.m_mesh.colors = colors.ToArray();
                        segments2.m_mesh.colors32 = colors32.ToArray();
                        segments2.m_mesh.tangents = tangents.ToArray();

                        nodes3.m_mesh.colors = colors.ToArray();
                        nodes3.m_mesh.colors32 = colors32.ToArray();
                        nodes3.m_mesh.tangents = tangents.ToArray();

                        nodes6.m_mesh.colors = colors.ToArray();
                        nodes6.m_mesh.colors32 = colors32.ToArray();
                        nodes6.m_mesh.tangents = tangents.ToArray();

                        nodes7.m_mesh.colors = colors.ToArray();
                        nodes7.m_mesh.colors32 = colors32.ToArray();
                        nodes7.m_mesh.tangents = tangents.ToArray();

                        info.m_segments = new[] { segments0, segments1, segments2 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3, nodes4, nodes5, nodes6, nodes7 };
                        break;
                    }

                    //    case NetInfoVersion.Elevated:
                    //    case NetInfoVersion.Bridge:
                    //        {
                    //            var segments0 = info.m_segments[0].ShallowClone();
                    //            var nodes0 = info.m_nodes[0].ShallowClone();
                    //            var nodes1 = info.m_nodes[0].ShallowClone();

                    //            segments0
                    //                .SetFlagsDefault()
                    //                .SetMeshes
                    //                (@"Roads\Highways\Meshes\16m\Elevated.obj",
                    //                 @"Roads\Highways\Meshes\16m\Elevated_LOD.obj");

                    //            nodes0
                    //                .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                    //                .SetMeshes
                    //                (@"Roads\Highways\Meshes\16m\Elevated_Node.obj",
                    //                 @"Roads\Highways\Meshes\16m\Elevated_Node_LOD.obj");

                    //            nodes1
                    //                .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                    //                .SetMeshes
                    //                (@"Roads\Highways\Meshes\16m\Elevated_Trans.obj",
                    //                 @"Roads\Highways\Meshes\16m\Elevated_Trans_LOD.obj");

                    //            info.m_segments = new[] { segments0 };
                    //            info.m_nodes = new[] { nodes0, nodes1 };
                    //        }
                    //        break;

                    //    case NetInfoVersion.Slope:
                    //        {
                    //            var segments0 = info.m_segments[0].ShallowClone();
                    //            var segments1 = info.m_segments[1].ShallowClone();
                    //            var segments2 = info.m_segments[1].ShallowClone();
                    //            var nodes0 = info.m_nodes[0].ShallowClone();
                    //            var nodes1 = info.m_nodes[0].ShallowClone();
                    //            var nodes2 = info.m_nodes[1].ShallowClone();
                    //            var nodes3 = info.m_nodes[1].ShallowClone();

                    //            segments0
                    //                .SetFlagsDefault()
                    //                .SetMeshes
                    //                (@"Roads\Highways\Meshes\16m\Tunnel_Gray.obj",
                    //                 @"Roads\Highways\Meshes\16m\Ground_LOD.obj");
                    //            segments1
                    //                .SetFlagsDefault();
                    //            segments2
                    //                .SetFlagsDefault()
                    //                .SetMeshes
                    //                (@"Roads\Highways\Meshes\16m\Slope.obj",
                    //                 @"Roads\Highways\Meshes\16m\Slope_LOD.obj");

                    //            nodes0
                    //                .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                    //                .SetMeshes
                    //                (@"Roads\Highways\Meshes\16m\Tunnel_Node_Gray.obj",
                    //                 @"Roads\Highways\Meshes\16m\Ground_Node_LOD.obj");
                    //            nodes1
                    //                .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                    //                .SetMeshes
                    //                (@"Roads\Highways\Meshes\16m\Slope_U_Node.obj",
                    //                 @"Roads\Highways\Meshes\16m\Slope_U_Node_LOD.obj");
                    //            nodes2
                    //                .SetFlags(NetNode.Flags.None, NetNode.Flags.UndergroundTransition)
                    //                .SetMeshes
                    //                (@"Roads\Highways\Meshes\16m\Ground_Node.obj",
                    //                 @"Roads\Highways\Meshes\16m\Ground_Node_LOD.obj");
                    //            nodes3
                    //                .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                    //                .SetMeshes
                    //                (@"Roads\Highways\Meshes\16m\Ground_Trans.obj",
                    //                 @"Roads\Highways\Meshes\16m\Ground_Trans_LOD.obj");

                    //            nodes1.m_material = defaultMaterial;

                    //            info.m_segments = new[] { segments0, segments1, segments2 };
                    //            info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3 };
                    //        }
                    //        break;

                    //case NetInfoVersion.Tunnel:
                    //    {
                    //        var segments0 = info.m_segments[0].ShallowClone();
                    //        var segments1 = info.m_segments[0].ShallowClone();
                    //        var nodes0 = info.m_nodes[0].ShallowClone();
                    //        var nodes1 = info.m_nodes[0].ShallowClone();

                    //        //segments0
                    //        //    //.SetFlagsDefault()
                    //        //    .SetMeshes
                    //        //    (@"Roads\Highways\Meshes\16m\Tunnel_Gray.obj",
                    //        //     @"Roads\Highways\Meshes\16m\Ground_LOD.obj");
                    //        segments1
                    //            //.SetFlagsDefault()
                    //            .SetMeshes
                    //            (@"PublicTransport\SubwayUtils\Meshes\10m\Tunnel.obj",
                    //             @"PublicTransport\SubwayUtils\Meshes\10m\Tunnel.obj");
                    //        nodes1
                    //            .SetMeshes
                    //            (@"PublicTransport\SubwayUtils\Meshes\10m\Tunnel_Node.obj",
                    //             @"PublicTransport\SubwayUtils\Meshes\10m\Tunnel_Node.obj");
                    //        //    .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                    //        //    .SetMeshes
                    //        //    (@"Roads\Highways\Meshes\16m\Tunnel_Node_Gray.obj",
                    //        //     @"Roads\Highways\Meshes\16m\Ground_Node_LOD.obj");
                    //        //nodes1
                    //        //    .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                    //        //    .SetMeshes
                    //        //    (@"Roads\Highways\Meshes\16m\Tunnel_Node.obj",
                    //        //     @"Roads\Highways\Meshes\16m\Tunnel_Node_LOD.obj");

                    //        segments1.m_material = defaultMaterial;
                    //        nodes1.m_material = defaultMaterial;

                    //        segments1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);
                    //        nodes1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);

                    //        info.m_segments = new[] { segments0, segments1 };
                    //        info.m_nodes = new[] { nodes0, nodes1 };
                    //    }
                    //    break;
            }
        }
    }
}
