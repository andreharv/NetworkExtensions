using System;
using System.Collections.Generic;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.PublicTransport.RailUtils
{
    public static partial class RailModels
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
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Pavement.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_LOD.obj");

                        segments2
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj");

                        nodes0
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Pavement_Node.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Pavement_Node_LOD.obj");

                        nodes1
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_Node_LOD.obj");
                        nodes3
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj");
                        nodes4
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_Start.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_Start_LOD.obj");
                        nodes5
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_End.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_End_LOD.obj");
                        nodes6
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power_Start.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power_Start.obj");
                        nodes7
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power_End.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power_End.obj");

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

                case NetInfoVersion.Elevated:
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
                        var nodes6 = info.m_nodes[2].ShallowClone();
                        var nodes7 = info.m_nodes[2].ShallowClone();

                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes2.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes4.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes7.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Elevated_Pavement.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Elevated_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_LOD.obj");

                        segments2
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj");

                        nodes0
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Elevated_Pavement.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Elevated_Pavement_Node_LOD.obj");

                        nodes1
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_Node_LOD.obj");
                        nodes2
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj");
                        nodes4
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_Start.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_Start_LOD.obj");
                        nodes5
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_End.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_End_LOD.obj");
                        nodes6
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power_Start.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power_Start.obj");
                        nodes7
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power_End.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power_End.obj");

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
                case NetInfoVersion.Bridge:
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
                        var nodes6 = info.m_nodes[2].ShallowClone();
                        var nodes7 = info.m_nodes[2].ShallowClone();

                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes2.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes4.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes7.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Bridge_Pavement.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Bridge_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_LOD.obj");

                        segments2
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj");

                        nodes0
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Elevated_Pavement.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Elevated_Pavement_Node_LOD.obj");

                        nodes1
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_Node_LOD.obj");
                        nodes2
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj");
                        nodes4
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_Start.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_Start_LOD.obj");
                        nodes5
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_End.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_End_LOD.obj");
                        nodes6
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power_Start.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power_Start.obj");
                        nodes7
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power_End.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power_End.obj");

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

                        nodes6.m_mesh.colors = colors.ToArray();
                        nodes6.m_mesh.colors32 = colors32.ToArray();
                        nodes6.m_mesh.tangents = tangents.ToArray();

                        nodes7.m_mesh.colors = colors.ToArray();
                        nodes7.m_mesh.colors32 = colors32.ToArray();
                        nodes7.m_mesh.tangents = tangents.ToArray();

                        var segmentNormals = segments0.m_mesh.normals;
                        var segmentVertices = segments0.m_mesh.vertices;
                        colors = new List<UnityEngine.Color>();
                        colors32 = new List<UnityEngine.Color32>();

                        for (int i = 0; i < segments0.m_mesh.vertexCount; i++)
                        {
                            if (segmentNormals[i].y == 1 && segmentVertices[i].y == 0)
                            {
                                colors.Add(new UnityEngine.Color(255, 255, 255, 255));
                                colors32.Add(new UnityEngine.Color32(255, 255, 255, 255));
                            }
                            else
                            {
                                colors.Add(new UnityEngine.Color(255, 0, 255, 255));
                                colors32.Add(new UnityEngine.Color32(255, 0, 255, 255));
                            }
                        }
                        segments0.m_mesh.colors = colors.ToArray();
                        segments0.m_mesh.colors32 = colors32.ToArray();

                        segmentNormals = segments0.m_lodMesh.normals;
                        segmentVertices = segments0.m_lodMesh.vertices;
                        colors = new List<UnityEngine.Color>();
                        colors32 = new List<UnityEngine.Color32>();

                        for (int i = 0; i < segments0.m_lodMesh.vertexCount; i++)
                        {
                            if (segmentNormals[i].y == 1 && Math.Abs(segmentVertices[i].x) <= 3 && segmentVertices[i].y < 3)
                            {
                                colors.Add(new UnityEngine.Color(255, 255, 255, 255));
                                colors32.Add(new UnityEngine.Color32(255, 255, 255, 255));
                            }
                            else
                            {
                                colors.Add(new UnityEngine.Color(255, 0, 255, 255));
                                colors32.Add(new UnityEngine.Color32(255, 0, 255, 255));
                            }
                        }
                        segments0.m_lodMesh.colors = colors.ToArray();
                        segments0.m_lodMesh.colors32 = colors32.ToArray();

                        info.m_segments = new[] { segments0, segments1, segments2 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3, nodes4, nodes5, nodes6, nodes7 };
                        break;
                    }

                case NetInfoVersion.Slope:
                    {
                        var pedSlope = Prefabs.Find<NetInfo>(NetInfos.Vanilla.PED_PAVEMENT_SLOPE);
                        var segments0 = info.m_segments[0];
                        var segments1 = info.m_segments[1];
                        var segments2 = info.m_segments[2];
                        var segments3 = pedSlope.m_segments[0];
                        var segments4 = ttInfo.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0];
                        var nodes1 = info.m_nodes[1];
                        var nodes2 = info.m_nodes[2];
                        var nodes3 = info.m_nodes[3];
                        var nodes4 = info.m_nodes[4];
                        var nodes5 = info.m_nodes[1].ShallowClone();
                        var nodes6 = info.m_nodes[1].ShallowClone();
                        var nodes7 = info.m_nodes[2].ShallowClone();
                        var nodes8 = info.m_nodes[2].ShallowClone();

                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes2.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        nodes7.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes8.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

                        segments0
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Tunnel.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Tunnel_Node_LOD.obj");
                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_LOD.obj");

                        segments2
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj");
                        segments4
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Pavement.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Pavement_LOD.obj");

                        nodes0
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Tunnel_Node.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Tunnel_Node_LOD.obj");

                        nodes1
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_Node_LOD.obj");
                        nodes2
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power.obj");
                        nodes3
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Pavement_Node.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Pavement_Node_LOD.obj");
                        nodes5
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_Start.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_Start_LOD.obj");
                        nodes6
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_End.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Rail_End_LOD.obj");
                        nodes7
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power_Start.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power_Start.obj");
                        nodes8
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Ground_Power_End.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Ground_Power_End.obj");

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

                        nodes7.m_mesh.colors = colors.ToArray();
                        nodes7.m_mesh.colors32 = colors32.ToArray();
                        nodes7.m_mesh.tangents = tangents.ToArray();

                        nodes8.m_mesh.colors = colors.ToArray();
                        nodes8.m_mesh.colors32 = colors32.ToArray();
                        nodes8.m_mesh.tangents = tangents.ToArray();

                        info.m_segments = new[] { segments0, segments1, segments2, segments3, segments4 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3, nodes4, nodes5, nodes6, nodes7, nodes8 };
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segments0 = info.m_segments[0];
                        var nodes0 = info.m_nodes[0];

                        segments0
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Tunnel.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Tunnel_Node_LOD.obj");

                        nodes0
                            .SetMeshes
                            (@"PublicTransport\RailUtils\Meshes\10m\Tunnel_Node.obj",
                            @"PublicTransport\RailUtils\Meshes\10m\Tunnel_Node_LOD.obj");

                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0 };
                    }
                    break;
            }
        }
    }
}
