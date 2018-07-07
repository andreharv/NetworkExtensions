using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup16m2mSWTestMesh(this NetInfo info, NetInfoVersion version)
        {
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var highwaySlopeInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_SLOPE);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        

                        info.m_twistSegmentEnds = true;
                        info.m_requireDirectRenderers = true;
                        info.m_requireSegmentRenderers = true;
                        info.m_snapBuildingNodes = true;
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[2].ShallowClone();
                        var segment3 = info.m_segments[1].ShallowClone();
                        var segment4 = info.m_segments[1].ShallowClone();
                        var segment5 = info.m_segments[2].ShallowClone();
                        var segment6 = info.m_segments[1].ShallowClone();
                        var segment7 = info.m_segments[1].ShallowClone();

                        var node0 = info.m_nodes[0].ShallowClone();

                        segment0
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Ground.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        segment0.m_backwardForbidden = NetSegment.Flags.AsymBackward;
                        segment0.m_backwardRequired = NetSegment.Flags.Invert;
                        segment0.m_forwardForbidden = NetSegment.Flags.Invert | NetSegment.Flags.AsymBackward | NetSegment.Flags.AsymForward;
                        segment0.m_forwardRequired = NetSegment.Flags.None;

                        segment1
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Ground.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        segment1.m_backwardForbidden = NetSegment.Flags.AsymBackward | NetSegment.Flags.Invert;
                        segment1.m_backwardRequired = NetSegment.Flags.None;
                        segment1.m_forwardForbidden = NetSegment.Flags.Invert | NetSegment.Flags.AsymBackward;
                        segment1.m_forwardRequired = NetSegment.Flags.None;



                        segment2
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Ground.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        segment2.m_backwardForbidden = NetSegment.Flags.AsymForward | NetSegment.Flags.Invert;
                        segment2.m_backwardRequired = NetSegment.Flags.None;
                        segment2.m_forwardForbidden = NetSegment.Flags.Invert | NetSegment.Flags.AsymBackward;
                        segment2.m_forwardRequired = NetSegment.Flags.None;


                        // правильно
                        segment3
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Ground.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        segment3.m_backwardForbidden = NetSegment.Flags.AsymBackward | NetSegment.Flags.AsymForward;
                        segment3.m_backwardRequired = NetSegment.Flags.Invert;
                        segment3.m_forwardForbidden = NetSegment.Flags.Invert |NetSegment.Flags.AsymForward;
                        segment3.m_forwardRequired = NetSegment.Flags.None;


                        segment4
                         .SetMeshes
                             (@"Roads\Common\Meshes\16m\2mSW\Ground.obj",
                             @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        segment4.m_backwardForbidden = NetSegment.Flags.AsymBackward | NetSegment.Flags.AsymForward | NetSegment.Flags.Invert;
                        segment4.m_backwardRequired = NetSegment.Flags.None;
                        segment4.m_forwardForbidden = NetSegment.Flags.AsymBackward;
                        segment4.m_forwardRequired = NetSegment.Flags.Invert;

                        segment5
                         .SetMeshes
                             (@"Roads\Common\Meshes\16m\2mSW\Ground.obj",
                             @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        segment5.m_backwardForbidden = NetSegment.Flags.AsymBackward | NetSegment.Flags.AsymForward;
                        segment5.m_backwardRequired = NetSegment.Flags.None;
                        segment5.m_forwardForbidden = NetSegment.Flags.Invert | NetSegment.Flags.AsymBackward;
                        segment5.m_forwardRequired = NetSegment.Flags.Invert;
                        segment6
                         .SetMeshes
                             (@"Roads\Common\Meshes\16m\2mSW\Ground.obj",
                             @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        segment6.m_backwardForbidden = NetSegment.Flags.Untouchable;
                        segment6.m_backwardRequired = NetSegment.Flags.None;
                        segment6.m_forwardForbidden = NetSegment.Flags.None;
                        segment6.m_forwardRequired = NetSegment.Flags.None;
                        segment7
                         .SetMeshes
                             (@"Roads\Common\Meshes\16m\2mSW\Ground.obj",
                             @"Roads\Common\Meshes\16m\2mSW\Ground_LOD.obj");
                        segment7.m_backwardForbidden = NetSegment.Flags.StopBoth;
                        segment7.m_backwardRequired = NetSegment.Flags.None;
                        segment7.m_forwardForbidden = NetSegment.Flags.None;
                        segment7.m_forwardRequired = NetSegment.Flags.None;






                        node0
                            .SetMeshes
                                (@"Roads\Common\Meshes\16m\2mSW\Ground_Node.obj",
                                @"Roads\Common\Meshes\16m\2mSW\Ground_Node_LOD.obj");

                  //      RoadHelper.HandleAsymSegmentFlags(segment0);
                   //     RoadHelper.HandleAsymSegmentFlags(segment2);

                        info.m_segments = new[] { segment0, segment1, segment2, segment3, segment4, segment5, segment6, segment7 };
                        info.m_nodes = new[] { node0 };

                        break;
                    }
              
            }
            return info;
        }
    }
}