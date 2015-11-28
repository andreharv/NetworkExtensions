using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Highways
{
    public static partial class HighwayModels
    {
        public static void Setup32mMesh(this NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            if (version == NetInfoVersion.Ground)
            {
                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;

                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_forwardRequired = NetSegment.Flags.None;

                var nodes1 = nodes0.ShallowClone();

                nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                nodes0.m_flagsRequired = NetNode.Flags.None;

                nodes1.m_flagsForbidden = NetNode.Flags.None;
                nodes1.m_flagsRequired = NetNode.Flags.Transition;

                segments0.SetMeshes
                    (@"Highways\Common\Meshes\32m\Ground.obj",
                     @"Highways\Common\Meshes\32m\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"Highways\Common\Meshes\32m\Ground_Node.obj",
                     @"Highways\Common\Meshes\32m\Ground_Node_LOD.obj");

                nodes1.SetMeshes
                    (@"Highways\Common\Meshes\32m\Ground_Trans.obj",
                     @"Highways\Common\Meshes\32m\Ground_Trans_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }
            else if (version == NetInfoVersion.Elevated)
            {
                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;

                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_forwardRequired = NetSegment.Flags.None;

                var nodes1 = nodes0.ShallowClone();

                nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                nodes0.m_flagsRequired = NetNode.Flags.None;

                nodes1.m_flagsForbidden = NetNode.Flags.None;
                nodes1.m_flagsRequired = NetNode.Flags.Transition;

                segments0.SetMeshes
                    (@"Highways\Common\Meshes\28m\Elevated.obj",
                    @"Highways\Common\Meshes\28m\Elevated_LOD.obj");

                nodes0.SetMeshes
                    (@"Highways\Common\Meshes\28m\Elevated_Node.obj",
                    @"Highways\Common\Meshes\28m\Elevated_Node_LOD.obj");

                nodes1.SetMeshes
                    (@"Highways\Common\Meshes\28m\Elevated_Trans.obj",
                    @"Highways\Common\Meshes\28m\Elevated_Trans_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }
            else if (version == NetInfoVersion.Bridge)
            {
                var segments0 = info.m_segments[0];
                var segments1 = info.m_segments[1];
                var nodes0 = info.m_nodes[0];

                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;

                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_forwardRequired = NetSegment.Flags.None;

                var nodes1 = nodes0.ShallowClone();

                nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                nodes0.m_flagsRequired = NetNode.Flags.None;

                nodes1.m_flagsForbidden = NetNode.Flags.None;
                nodes1.m_flagsRequired = NetNode.Flags.Transition;

                segments0.SetMeshes
                    (@"Highways\Common\Meshes\28m\Elevated.obj",
                    @"Highways\Common\Meshes\28m\Elevated_LOD.obj");

                nodes0.SetMeshes
                    (@"Highways\Common\Meshes\28m\Elevated_Node.obj",
                    @"Highways\Common\Meshes\28m\Elevated_Node_LOD.obj");

                nodes1.SetMeshes
                    (@"Highways\Common\Meshes\28m\Elevated_Trans.obj",
                    @"Highways\Common\Meshes\28m\Elevated_Trans_LOD.obj");

                info.m_segments = new[] { segments0, segments1 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }
            else if (version == NetInfoVersion.Slope)
            {
                var segments0 = info.m_segments[0];
                var segments1 = info.m_segments[1];
                var segments2 = segments1.ShallowClone();
                var nodes0 = info.m_nodes[0];
                var nodes1 = info.m_nodes[1];
                var nodes2 = nodes0.ShallowClone();
                var nodes3 = nodes1.ShallowClone();

                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;

                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_forwardRequired = NetSegment.Flags.None;

                segments1.m_backwardForbidden = NetSegment.Flags.None;
                segments1.m_backwardRequired = NetSegment.Flags.None;

                segments1.m_forwardForbidden = NetSegment.Flags.None;
                segments1.m_forwardRequired = NetSegment.Flags.None;

                segments2.m_backwardForbidden = NetSegment.Flags.None;
                segments2.m_backwardRequired = NetSegment.Flags.None;

                segments2.m_forwardForbidden = NetSegment.Flags.None;
                segments2.m_forwardRequired = NetSegment.Flags.None;

                //nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                //nodes0.m_flagsRequired = NetNode.Flags.Underground;

                nodes1.m_flagsForbidden = NetNode.Flags.UndergroundTransition;
                nodes1.m_flagsRequired = NetNode.Flags.None;

                //nodes2.m_flagsForbidden = NetNode.Flags.None;
                //nodes2.m_flagsRequired = NetNode.Flags.UndergroundTransition;

                //nodes3.m_flagsForbidden = NetNode.Flags.Underground;
                //nodes3.m_flagsRequired = NetNode.Flags.Transition;

                segments0.SetMeshes
                    (@"Highways\Common\Meshes\32m\Tunnel_Node_Gray.obj");
                segments2.SetMeshes
                    (@"Highways\Common\Meshes\32m\Tunnel_Node_Gray.obj",
                     @"Highways\Common\Meshes\32m\Slope_LOD.obj");

                nodes0.SetMeshes
                    (@"Highways\Common\Meshes\32m\Tunnel_Node_Gray.obj",
                     @"Highways\Common\Meshes\32m\Ground_LOD.obj");
                nodes1.SetMeshes
                    (@"Highways\Common\Meshes\32m\Ground_Node.obj",
                    @"Highways\Common\Meshes\32m\Ground_Node_LOD.obj");
                nodes2.SetMeshes
                    (@"Highways\Common\Meshes\32m\Slope_U_Node.obj");
                nodes3.SetMeshes
                    (@"Highways\Common\Meshes\32m\Ground_Trans.obj",
                     @"Highways\Common\Meshes\32m\Ground_Trans_LOD.obj");

                nodes2.m_material = defaultMaterial;

                info.m_segments = new[] { segments0, segments1, segments2 };
                info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3 };
            }
            else if (version == NetInfoVersion.Tunnel)
            {
                var segments0 = info.m_segments[0];
                var segments1 = segments0.ShallowClone();
                var nodes0 = info.m_nodes[0];
                var nodes1 = nodes0.ShallowClone();
                //var nodes2 = nodes1.ShallowClone();

                //segments1.m_backwardForbidden = NetSegment.Flags.None;
                //segments1.m_backwardRequired = NetSegment.Flags.None;

                //segments1.m_forwardForbidden = NetSegment.Flags.None;
                //segments1.m_forwardRequired = NetSegment.Flags.None;

                nodes0.m_flagsForbidden = NetNode.Flags.None;
                nodes0.m_flagsRequired = NetNode.Flags.None;

                nodes1.m_flagsForbidden = NetNode.Flags.None;
                nodes1.m_flagsRequired = NetNode.Flags.None;

                //nodes2.m_flagsForbidden = NetNode.Flags.None;
                // nodes2.m_flagsRequired = NetNode.Flags.UndergroundTransition;

                segments0.SetMeshes
                    (@"Highways\Common\Meshes\32m\Tunnel_Gray.obj",
                    @"Highways\Common\Meshes\32m\Ground_LOD.obj");
                segments1.SetMeshes
                    (@"Highways\Common\Meshes\32m\Tunnel.obj",
                    @"Highways\Common\Meshes\32m\Tunnel_LOD.obj");
                nodes0.SetMeshes
                     (@"Highways\Common\Meshes\32m\Tunnel_Node_Gray.obj",
                     @"Highways\Common\Meshes\32m\Ground_LOD.obj");
                nodes1.SetMeshes
                    (@"Highways\Common\Meshes\32m\Tunnel_Node.obj");
                // nodes2.SetMeshes
                //    (@"Highways\Common\Meshes\32m\Tunnel.obj",
                //    @"Highways\Common\Meshes\32m\Ground_LOD.obj");

                segments1.m_material = defaultMaterial;
                nodes1.m_material = defaultMaterial;
                //nodes2.m_material = defaultMaterial;

                segments1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);
                nodes1.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);
                //nodes2.m_surfaceMapping = new UnityEngine.Vector4(0, 0, 0, 0);

                info.m_segments = new[] { segments0, segments1 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }
        }
    }
}
