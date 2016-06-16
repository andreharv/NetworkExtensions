using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var ttStationInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.TRAIN_STATION_TRACK);
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
                        var nodes8 = info.m_nodes[1].ShallowClone();
                        var nodes9 = info.m_nodes[1].ShallowClone();
                        var nodes10 = info.m_nodes[3].ShallowClone();
                        var nodes11 = info.m_nodes[3].ShallowClone();
                        
                        var nodes12 = ttStationInfo.m_nodes[1].ShallowClone();
                        var nodes13 = ttStationInfo.m_nodes[3].ShallowClone();

                        nodes1.m_connectGroup = NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.Oneway;
                        nodes3.m_connectGroup = NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.Oneway;
                        nodes4.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes7.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        nodes8.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes9.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayEnd;
                        nodes10.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes11.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayEnd;

                        nodes12.m_connectGroup = NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.Oneway;
                        nodes13.m_connectGroup = NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.Oneway;

                        nodes4
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\RailStart.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\6m\Ground_Rail_Start_LOD.obj");
                        nodes5
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\RailEnd.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\6m\Ground_Rail_End_LOD.obj");
                        nodes6
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Power_Start.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\6m\Ground_Power_Start.obj");
                        nodes7
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\10m\Ground_Power_End.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\6m\Ground_Power_End.obj");

                        nodes8
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\6m\Station\Ground_Station_Rail_End.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\6m\Ground_Rail_Start_LOD.obj");
                        nodes9
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\6m\Station\Ground_Station_Rail_Start.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\6m\Ground_Rail_End_LOD.obj");
                        nodes10
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\6m\Station\Ground_Power_End.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\6m\Station\Ground_Power_End.obj");
                        nodes11
                            .SetMeshes
                            (@"PublicTransport\Rail\RailUtils\Meshes\6m\Station\Ground_Power_Start.obj",
                            @"PublicTransport\Rail\RailUtils\Meshes\6m\Station\Ground_Power_Start.obj");

                        var colors = new List<UnityEngine.Color>();
                        var colors32 = new List<UnityEngine.Color32>();
                        var tangents = new List<UnityEngine.Vector4>();

                        for (int i = 0; i < segments2.m_mesh.vertexCount; i++)
                        {
                            colors.Add(new UnityEngine.Color(0, 0, 0, 255));
                            colors32.Add(new UnityEngine.Color32(0, 0, 0, 255));
                            tangents.Add(new UnityEngine.Vector4(0, 0, 1, -1));
                        }

                        var colorsb = new List<UnityEngine.Color>();
                        var colors32b = new List<UnityEngine.Color32>();
                        var tangentsb = new List<UnityEngine.Vector4>();

                        for (int i = 0; i < nodes6.m_mesh.vertexCount; i++)
                        {
                            colorsb.Add(new UnityEngine.Color(0, 0, 0, 255));
                            colors32b.Add(new UnityEngine.Color32(0, 0, 0, 255));
                            tangentsb.Add(new UnityEngine.Vector4(0, 0, 1, -1));
                        }

                        segments2.m_mesh.colors = colors.ToArray();
                        segments2.m_mesh.colors32 = colors32.ToArray();
                        segments2.m_mesh.tangents = tangents.ToArray();

                        nodes3.m_mesh.colors = colors.ToArray();
                        nodes3.m_mesh.colors32 = colors32.ToArray();
                        nodes3.m_mesh.tangents = tangents.ToArray();

                        nodes6.m_mesh.colors = colorsb.ToArray();
                        nodes6.m_mesh.colors32 = colors32b.ToArray();
                        nodes6.m_mesh.tangents = tangentsb.ToArray();

                        nodes7.m_mesh.colors = colorsb.ToArray();
                        nodes7.m_mesh.colors32 = colors32b.ToArray();
                        nodes7.m_mesh.tangents = tangentsb.ToArray();

                        nodes10.m_mesh.colors = colors.ToArray();
                        nodes10.m_mesh.colors32 = colors32.ToArray();
                        nodes10.m_mesh.tangents = tangents.ToArray();

                        nodes11.m_mesh.colors = colors.ToArray();
                        nodes11.m_mesh.colors32 = colors32.ToArray();
                        nodes11.m_mesh.tangents = tangents.ToArray();

                        info.m_segments = new[] { segments0, segments1, segments2
};
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3, nodes4, nodes5, nodes6, nodes7, nodes8, nodes9, nodes10, nodes11, nodes12, nodes13 };
                        break;
                    }
            }
        }
    }
}