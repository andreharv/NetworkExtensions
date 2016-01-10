using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup32m3mSW2mMdnMesh(this NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0];
                        var nodes0 = info.m_nodes[0];

                        segments0
                        .SetFlagsDefault()
                        .SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2mMdn\Ground.obj");

                        nodes0.SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2mMdn\Ground_Node.obj");

                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0 };
                        break;
                    }
            }
            return info;
        }
    }
}