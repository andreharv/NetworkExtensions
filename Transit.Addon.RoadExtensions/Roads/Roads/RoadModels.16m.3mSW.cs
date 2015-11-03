using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Roads.Roads
{
    public static partial class RoadModels
    {
        public static void Setup16m3mSWMesh(this NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0];
                        var nodes0 = info.m_nodes[0];

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                                (@"Roads\Roads\Meshes\16m\3mSW\Elevated.obj",
                                @"Roads\Roads\Meshes\16m\3mSW\Elevated_LOD.obj");

                        nodes0.SetMeshes
                            (@"Roads\Roads\Meshes\16m\3mSW\Elevated_Node.obj",
                            @"Roads\Roads\Meshes\16m\3mSW\Elevated_Node_LOD.obj");

                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0 };
                        break;
                    }
            }
        }
    }
}