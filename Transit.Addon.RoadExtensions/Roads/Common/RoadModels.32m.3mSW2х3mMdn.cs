using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup32m3mSW2x3mMdnMesh(this NetInfo info, NetInfoVersion version)
        {
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            var slopeInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_4L_SLOPE);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            switch (version)
            {
                case NetInfoVersion.Ground:
                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments2 = info.m_segments[2].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();

                        segments0
                        .SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2x3mMdn\Ground.obj");
                        segments1
                        .SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2x3mMdn\Ground_Bus.obj");
                        segments2
                        .SetMeshes
                            (@"Roads\Common\Meshes\32m\3mSW2x3mMdn\Ground_BusBoth.obj");

                        nodes0
                        .SetMeshes(@"Roads\Common\Meshes\32m\3mSW2x3mMdn\Ground_Node.obj");

                        info.m_segments = new[] { segments0, segments1, segments2 };
                        info.m_nodes = new[] { nodes0 };
                        break;
                    }
            }
            return info;
        }
    }
}