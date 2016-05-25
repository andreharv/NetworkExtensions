using System.Collections.Generic;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup8mNoSwWoodMesh(this NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0];
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[0].ShallowClone();

                        nodes0.m_flagsRequired = NetNode.Flags.None;
                        nodes0.m_flagsForbidden = NetNode.Flags.Transition;

                        nodes1.m_flagsRequired = NetNode.Flags.Transition;
                        nodes1.m_flagsForbidden = NetNode.Flags.None;

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSW\Ground.obj",
                            @"Roads\Common\Meshes\8m\NoSW\Ground_LOD.obj");

                        nodes0
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSW\Ground_Node.obj",
                            @"Roads\Common\Meshes\8m\NoSW\Ground_Node_LOD.obj");
                        nodes1
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSW\Ground_Trans.obj",
                            @"Roads\Common\Meshes\8m\NoSW\Ground_Trans_LOD.obj");

                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0];
                        var nodes0 = info.m_nodes[0];
                        var nodes1 = info.m_nodes[0].ShallowClone();

                        nodes0.m_flagsRequired = NetNode.Flags.None;
                        nodes0.m_flagsForbidden = NetNode.Flags.Transition;

                        nodes1.m_flagsRequired = NetNode.Flags.Transition;
                        nodes1.m_flagsForbidden = NetNode.Flags.None;

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSwWood\Elevated.obj",
                            @"Roads\Common\Meshes\8m\NoSwWood\Elevated_LOD.obj");

                        nodes0
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSwWood\Elevated_Node.obj",
                            @"Roads\Common\Meshes\8m\NoSwWood\Elevated_Node_LOD.obj");

                        nodes1
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSW\Elevated_Trans.obj",
                            @"Roads\Common\Meshes\8m\NoSW\Elevated_Trans_LOD.obj");
                        var colors = new List<UnityEngine.Color>();
                        var colors32 = new List<UnityEngine.Color32>();
                        for (int i = 0; i < segments0.m_mesh.vertexCount; i++)
                        {
                            colors.Add(new UnityEngine.Color(255, 0, 255, 255));
                            colors32.Add(new UnityEngine.Color32(255, 0, 255, 255));
                        }
                        segments0.m_mesh.colors = colors.ToArray();
                        segments0.m_mesh.colors32 = colors32.ToArray();

                        colors = new List<UnityEngine.Color>();
                        colors32 = new List<UnityEngine.Color32>();
                        for (int i = 0; i < segments0.m_lodMesh.vertexCount; i++)
                        {
                            colors.Add(new UnityEngine.Color(255, 0, 255, 255));
                            colors32.Add(new UnityEngine.Color32(255, 0, 255, 255));
                        }
                        segments0.m_lodMesh.colors = colors.ToArray();
                        segments0.m_lodMesh.colors32 = colors32.ToArray();

                        colors = new List<UnityEngine.Color>();
                        colors32 = new List<UnityEngine.Color32>();
                        for (int i = 0; i < nodes0.m_mesh.vertexCount; i++)
                        {
                            colors.Add(new UnityEngine.Color(255, 0, 255, 255));
                            colors32.Add(new UnityEngine.Color32(255, 0, 255, 255));
                        }
                        nodes0.m_mesh.colors = colors.ToArray();
                        nodes0.m_mesh.colors32 = colors32.ToArray();

                        colors = new List<UnityEngine.Color>();
                        colors32 = new List<UnityEngine.Color32>();
                        for (int i = 0; i < nodes0.m_lodMesh.vertexCount; i++)
                        {
                            colors.Add(new UnityEngine.Color(255, 0, 255, 255));
                            colors32.Add(new UnityEngine.Color32(255, 0, 255, 255));
                        }
                        nodes0.m_lodMesh.colors = colors.ToArray();
                        nodes0.m_lodMesh.colors32 = colors32.ToArray();

                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                        break;
                    }
            }
            return info;
        }
    }
}