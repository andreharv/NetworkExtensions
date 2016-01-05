using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Highways
{
    public static partial class HighwayModels
    {
        public static void Setup10mMesh(this NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_RAMP);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            switch (version)
            {
                case NetInfoVersion.Slope:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = info.m_segments[1];

                        var nodes0 = info.m_nodes[0];
                        var nodes1 = info.m_nodes[1];
                        var nodes2 = info.m_nodes[2];
                        var nodes3 = highwayInfo.m_nodes[0].ShallowClone();

                        nodes3
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\10m\Slope_U_Node.obj",
                             @"Roads\Highways\Common\Meshes\10m\Slope_U_Node_LOD.obj");

                        nodes3.m_material = defaultMaterial;

                        info.m_segments = new[] { segments0, segments1 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3 };
                    }
                    break;

                case NetInfoVersion.Tunnel:
                    {
                        var segments0 = info.m_segments[0];
                        var segments1 = highwayInfo.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0];
                        var nodes1 = highwayInfo.m_nodes[0].ShallowClone();

                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\10m\Tunnel.obj",
                             @"Roads\Highways\Common\Meshes\10m\Tunnel_LOD.obj");

                        nodes1
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Roads\Highways\Common\Meshes\10m\Tunnel_Node.obj",
                             @"Roads\Highways\Common\Meshes\10m\Tunnel_Node_LOD.obj");

                        segments1.m_material = defaultMaterial;

                        nodes1.m_material = defaultMaterial;

                        segments1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);
                        nodes1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);


                        info.m_segments = new[] { segments0, segments1 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                    }
                    break;
            }
        }
    }
}
