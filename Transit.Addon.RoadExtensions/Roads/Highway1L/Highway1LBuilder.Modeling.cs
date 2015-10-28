using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Roads.Highway1L
{
    public partial class Highway1LBuilder
    {
        private static void SetupModels(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0];
                        var nodes0 = info.m_nodes[0];
                        var nodes1 = nodes0.ShallowClone();

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Ground.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Ground_LOD.obj");

                        nodes0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Ground_Node.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Ground_Node_LOD.obj");

                        nodes1
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Ground_Trans.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Ground_Trans_LOD.obj");

                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                    }
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0];
                        var nodes0 = info.m_nodes[0];
                        var nodes1 = nodes0.ShallowClone();

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Elevated.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Elevated_LOD.obj");

                        nodes0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Elevated_Node.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Elevated_Node_LOD.obj");

                        nodes1
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Elevated_Trans.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Elevated_Trans_LOD.obj");

                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                    }
                    break;

                case NetInfoVersion.Slope:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = info.m_segments[1];
                        var segments2 = segments1.ShallowClone();
                        var nodes0 = info.m_nodes[0];
                        var nodes1 = info.m_nodes[1];
                        var nodes2 = nodes0.ShallowClone();
                        var nodes3 = nodes1.ShallowClone();

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel_Gray.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Ground_LOD.obj");
                        segments1
                            .SetFlagsDefault();
                        segments2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Slope.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Slope_LOD.obj");

                        nodes0
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel_Node_Gray.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Ground_Node_LOD.obj");
                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.UndergroundTransition)
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Ground_Node.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Ground_Node_LOD.obj");
                        nodes2
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Slope_U_Node.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Slope_U_Node_LOD.obj");
                        nodes3
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Ground_Trans.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Ground_Trans_LOD.obj");

                        nodes2.m_material = defaultMaterial;

                        info.m_segments = new[] { segments0, segments1, segments2 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3 };
                    }
                    break;

                case NetInfoVersion.Tunnel:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = segments0.ShallowClone();
                        var nodes0 = info.m_nodes[0];
                        var nodes1 = nodes0.ShallowClone();
                        //var nodes2 = nodes1.ShallowClone();

                        segments0
                            //.SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel_Gray.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Ground_LOD.obj");
                        segments1
                            //.SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Tunnel_LOD.obj");
                        nodes0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel_Node_Gray.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Ground_Node_LOD.obj");
                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            //.SetFlags(NetNode.Flags.Transition, NetNode.Flags.Underground)
                            .SetMeshes
                            (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel_Node.obj",
                             @"Roads\aHighwayTemplates\Meshes\16m\Tunnel_Node_LOD.obj");

                        // nodes2
                        //    .SetFlags(NetNode.Flags.None, NetNode.Flags.UndergroundTransition)
                        //    .SetMeshes
                        //    (@"Roads\aHighwayTemplates\Meshes\16m\Tunnel.obj",
                        //    @"Roads\aHighwayTemplates\Meshes\16m\Ground_LOD.obj");

                        segments1.m_material = defaultMaterial;
                        nodes1.m_material = defaultMaterial;
                        //nodes2.m_material = defaultMaterial;

                        segments1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);
                        nodes1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);
                        //nodes2.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);

                        info.m_segments = new[] { segments0, segments1 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                    }
                    break;
            }
        }
    }
}
