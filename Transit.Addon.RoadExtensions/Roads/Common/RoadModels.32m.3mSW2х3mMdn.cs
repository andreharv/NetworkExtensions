using Transit.Framework;
using Transit.Framework.Network;

namespace TransitPlus.Addon.RoadExtensions.Roads.Common
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
                            var segments0 = info.m_segments[0];
                        var nodes0 = info.m_nodes[0];

                            segments0
                            .SetFlagsDefault()
                            .SetMeshes
                                (@"Roads\Common\Meshes\32m\3mSW2x3mMdn\Ground.obj");



                            segments0.m_backwardForbidden = NetSegment.Flags.None;
                            segments0.m_backwardRequired = NetSegment.Flags.None;
                            segments0.m_forwardForbidden = NetSegment.Flags.None;
                            segments0.m_forwardRequired = NetSegment.Flags.None;



                        info.m_nodes[0].SetMeshes(@"Roads\Common\Meshes\32m\3mSW2x3mMdn\Ground_Node.obj");
                                var nodes1 = info.m_nodes[0].ShallowClone();
                                nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                                nodes1.m_flagsRequired = NetNode.Flags.Transition;

                                info.m_segments = new[] { segments0 };
                                info.m_nodes = new[] { nodes0, nodes1};
                                break;
                            }

                    }
            return info;
        }
    }
}