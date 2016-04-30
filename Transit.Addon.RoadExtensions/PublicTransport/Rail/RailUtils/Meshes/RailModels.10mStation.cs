using System;
using System.Collections.Generic;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.PublicTransport.RailUtils
{
    public static partial class RailModels
    {
        public static void Setup10mStationMesh(this NetInfo info, NetInfoVersion version)
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
                        var nodes5 = info.m_nodes[3].ShallowClone();
                        var nodes6 = info.m_nodes[1].ShallowClone();
                        var nodes7 = info.m_nodes[3].ShallowClone();

                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes3.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes4.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        nodes7.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Pavement.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Rail.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Rail_LOD.obj");

                        segments2
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Power.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Power.obj");

                        nodes0
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Pavement_Node.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Pavement_Node_LOD.obj");

                        nodes1
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Rail.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Rail_Node_LOD.obj");

                        nodes2
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Level_Crossing.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Level_Crossing_LOD.obj");

                        nodes3
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Power.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Power.obj");
                        nodes4
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Station\Ground_Station_Rail_Start.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Rail_End_LOD.obj");
                        nodes5
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Station\Ground_Power_Start.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Station\Ground_Power_Start.obj");
                        nodes6
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Station\Ground_Station_Rail_End.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Rail_End_LOD.obj");
                        nodes7
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Station\Ground_Power_End.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Station\Ground_Power_End.obj");

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

                        nodes5.m_mesh.colors = colors.ToArray();
                        nodes5.m_mesh.colors32 = colors32.ToArray();
                        nodes5.m_mesh.tangents = tangents.ToArray();

                        nodes7.m_mesh.colors = colors.ToArray();
                        nodes7.m_mesh.colors32 = colors32.ToArray();
                        nodes7.m_mesh.tangents = tangents.ToArray();

                        info.m_segments = new[] { segments0, segments1, segments2 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3, nodes4, nodes5, nodes6, nodes7 };
                        break;
                    }

                case NetInfoVersion.Elevated:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = info.m_segments[1];
                        var segments2 = info.m_segments[2];
                        var nodes0 = info.m_nodes[0];
                        var nodes1 = info.m_nodes[1];
                        var nodes2 = info.m_nodes[2];
                        var nodes3 = info.m_nodes[3];
                        var nodes4 = info.m_nodes[2].ShallowClone();
                        var nodes5 = info.m_nodes[1].ShallowClone();

                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes2.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes4.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.Oneway;
                        nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.Oneway;

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Elevated_Pavement.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Elevated_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Rail.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Rail_LOD.obj");

                        segments2
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Power.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Power.obj");

                        nodes0
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Elevated_Pavement.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Elevated_Pavement_Node_LOD.obj");

                        nodes1
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Rail.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Rail_Node_LOD.obj");
                        nodes2
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Power.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Power.obj");
                        nodes4
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Station\Ground_Power_StartEnd.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Rail_Start_LOD.obj");
                        nodes5
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Station\Ground_Rail_StartEnd.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Rail_End_LOD.obj");

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

                        nodes2.m_mesh.colors = colors.ToArray();
                        nodes2.m_mesh.colors32 = colors32.ToArray();
                        nodes2.m_mesh.tangents = tangents.ToArray();

                        nodes4.m_mesh.colors = colors.ToArray();
                        nodes4.m_mesh.colors32 = colors32.ToArray();
                        nodes4.m_mesh.tangents = tangents.ToArray();


                        info.m_segments = new[] { segments0, segments1, segments2 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3, nodes4, nodes5 };
                        break;
                    }
            }
        }
    }
}
