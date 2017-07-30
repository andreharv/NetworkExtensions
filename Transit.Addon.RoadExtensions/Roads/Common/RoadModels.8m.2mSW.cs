using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup8m2mSWMesh(this NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    { 
                        var segments0 = info.m_segments[0];
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Common\Meshes\8m\2mSW\Ground.obj",
                             @"Roads\Common\Meshes\8m\2mSW\Ground_LOD.obj");

                        nodes0
                            .SetMeshes
                            (@"Roads\Common\Meshes\8m\2mSW\Ground_Node.obj",
                             @"Roads\Common\Meshes\8m\2mSW\Ground_Node.obj");
                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0 };
                        break;
                    }
            }
            return info;
        }
    }
}