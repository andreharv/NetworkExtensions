using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Roads.Highways
{
    public static partial class HighwayModels
    {
        public static void Setup16mMesh(this NetInfo info, NetInfoVersion version)
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
                        var segments0 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[0].ShallowClone();

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Ground.obj",
                             @"Roads\Highways\Common\Meshes\16m\Ground_LOD.obj");

                        nodes0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Ground_Node.obj",
                             @"Roads\Highways\Common\Meshes\16m\Ground_Node_LOD.obj");

                        nodes1
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Ground_Trans.obj",
                             @"Roads\Highways\Common\Meshes\16m\Ground_Trans_LOD.obj");

                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                    }
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[0].ShallowClone();

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Elevated.obj",
                             @"Roads\Highways\Common\Meshes\16m\Elevated_LOD.obj");

                        nodes0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Elevated_Node.obj",
                             @"Roads\Highways\Common\Meshes\16m\Elevated_Node_LOD.obj");

                        nodes1
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Elevated_Trans.obj",
                             @"Roads\Highways\Common\Meshes\16m\Elevated_Trans_LOD.obj");

                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                    }
                    break;

                case NetInfoVersion.Slope:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments2 = info.m_segments[1].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[0].ShallowClone();
                        var nodes2 = info.m_nodes[1].ShallowClone();
                        var nodes3 = info.m_nodes[1].ShallowClone();

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Tunnel_Gray.obj",
                             @"Roads\Highways\Common\Meshes\16m\Ground_LOD.obj");
                        segments1
                            .SetFlagsDefault();
                        segments2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Slope.obj",
                             @"Roads\Highways\Common\Meshes\16m\Slope_LOD.obj");

                        nodes0
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Tunnel_Node_Gray.obj",
                             @"Roads\Highways\Common\Meshes\16m\Ground_Node_LOD.obj");
                        nodes1
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Slope_U_Node.obj",
                             @"Roads\Highways\Common\Meshes\16m\Slope_U_Node_LOD.obj");
                        nodes2
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.UndergroundTransition)
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Ground_Node.obj",
                             @"Roads\Highways\Common\Meshes\16m\Ground_Node_LOD.obj");
                        nodes3
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Ground_Trans.obj",
                             @"Roads\Highways\Common\Meshes\16m\Ground_Trans_LOD.obj");

                        nodes1.m_material = defaultMaterial;

                        info.m_segments = new[] { segments0, segments1, segments2 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3 };
                    }
                    break;

                case NetInfoVersion.Tunnel:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[0].ShallowClone();

                        segments0
                            //.SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Tunnel_Gray.obj",
                             @"Roads\Highways\Common\Meshes\16m\Ground_LOD.obj");
                        segments1
                            //.SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Tunnel.obj",
                             @"Roads\Highways\Common\Meshes\16m\Tunnel_LOD.obj");
                        nodes0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Tunnel_Node_Gray.obj",
                             @"Roads\Highways\Common\Meshes\16m\Ground_Node_LOD.obj");
                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\16m\Tunnel_Node.obj",
                             @"Roads\Highways\Common\Meshes\16m\Tunnel_Node_LOD.obj");

                        segments1.m_material = defaultMaterial;
                        nodes1.m_material = defaultMaterial;

                        //segments1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);
                        //nodes1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);

                        info.m_segments = new[] { segments0, segments1 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                    }
                    break;
            }
        }
    }
}
