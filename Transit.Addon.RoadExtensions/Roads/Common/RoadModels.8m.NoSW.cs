using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup8mNoSWMesh(this NetInfo info, NetInfoVersion version)
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
                            @"Roads\Common\Meshes\8m\NoSW\Elevated.obj",
                            @"Roads\Common\Meshes\8m\NoSW\Elevated_LOD.obj");

                        nodes0
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSW\Elevated_Node.obj",
                            @"Roads\Common\Meshes\8m\NoSW\Elevated_Node_LOD.obj");

                        nodes1
                            .SetMeshes(
                            @"Roads\Common\Meshes\8m\NoSW\Elevated_Trans.obj",
                            @"Roads\Common\Meshes\8m\NoSW\Elevated_Trans_LOD.obj");

                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                        break;
                    }
            }
            return info;
        }
    }
}