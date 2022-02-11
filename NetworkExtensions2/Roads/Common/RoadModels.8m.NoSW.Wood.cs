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
                        var segments0 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[0].ShallowClone();
                        var nodes2 = info.m_nodes[0].ShallowClone();
                        var nodes3 = info.m_nodes[0].ShallowClone();
                        var nodes4 = info.m_nodes[0].ShallowClone();
                        nodes0.m_flagsRequired = NetNode.Flags.None;
                        nodes0.m_flagsForbidden = NetNode.Flags.Transition;

                        nodes1.m_flagsRequired = NetNode.Flags.Transition;
                        nodes1.m_flagsForbidden = NetNode.Flags.None;

                        nodes2.m_flagsRequired = NetNode.Flags.None;
                        nodes2.m_flagsForbidden = NetNode.Flags.Transition | NetNode.Flags.LevelCrossing;

                        nodes3.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        nodes3.m_flagsForbidden = NetNode.Flags.None;

                        nodes4.m_flagsRequired = NetNode.Flags.Transition;
                        nodes4.m_flagsForbidden = NetNode.Flags.None;

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSwWood\Ground_NoCom.obj",
                            @"Roads\Common\Meshes\8m\NoSwWood\Ground_LOD_NoCom.obj")
                            .SetConsistentUVs();
                        //nodes0
                        //    .SetMeshes(
                        //    @"Roads\Common\Meshes\8m\NoSwWood\Ground_Node_NoCom.obj",
                        //    @"Roads\Common\Meshes\8m\NoSwWood\Ground_Node_LOD_NoCom.obj")
                        //    .SetConsistentUVs();
                        nodes0
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSW\Ground_Node.obj",
                            @"Roads\Common\Meshes\8m\NoSW\Ground_Node_LOD.obj");
                        nodes1
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSW\Ground_Trans.obj",
                            @"Roads\Common\Meshes\8m\NoSW\Ground_Trans_LOD.obj");
                        nodes2
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSwWood\Ground_Node_NoCom.obj",
                            @"Roads\Common\Meshes\8m\NoSwWood\Ground_Node_LOD_NoCom.obj")
                            .SetConsistentUVs();
                        nodes3
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSwWood\Ground_LevelCrossing_NoCom.obj",
                            @"Roads\Common\Meshes\8m\NoSwWood\Ground_LevelCrossing_NoCom.obj")
                            .SetConsistentUVs();
                        nodes4
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSwWood\Ground_Trans.obj",
                            @"Roads\Common\Meshes\8m\NoSwWood\Ground_Trans_LOD.obj");

                        nodes2.m_directConnect = true;      
                        nodes2.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        nodes3.m_directConnect = true;
                        nodes3.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2,nodes3,nodes4 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[0].ShallowClone();

                        nodes0.m_flagsRequired = NetNode.Flags.None;
                        nodes0.m_flagsForbidden = NetNode.Flags.Transition;

                        nodes1.m_flagsRequired = NetNode.Flags.Transition;
                        nodes1.m_flagsForbidden = NetNode.Flags.None;

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSwWood\Elevated_NoCom.obj",
                            @"Roads\Common\Meshes\8m\NoSwWood\Elevated_LOD_NoCom.obj")
                            .SetConsistentUVs();

                        nodes0
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSwWood\Elevated_Node_NoCom.obj",
                            @"Roads\Common\Meshes\8m\NoSwWood\Elevated_Node_LOD_NoCom.obj")
                            .SetConsistentUVs();

                        nodes1
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSwWood\Elevated_Trans.obj",
                            @"Roads\Common\Meshes\8m\NoSW\Elevated_Trans_LOD.obj");
                        //nodes2
                        //    .SetMeshes(
                        //    @"Roads\Common\Meshes\8m\NoSwWood\Ground_Node_NoCom.obj",
                        //    @"Roads\Common\Meshes\8m\NoSwWood\Ground_Node_LOD_NoCom.obj")
                        //    .SetConsistentUVs();

                        //nodes2.m_directConnect = true;
                        //nodes2.m_connectGroup = NetInfo.ConnectGroup.CenterTram;

                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0, nodes1};
                        break;
                    }
            }
            return info;
        }
    }
}