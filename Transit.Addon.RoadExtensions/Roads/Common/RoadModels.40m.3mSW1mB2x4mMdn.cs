using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup40m3mSW1mB2x4mMdnMesh(this NetInfo info, NetInfoVersion version)
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
                            (@"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground.obj",
                            @"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Node_LOD.obj");

                        var segment1 = segments0.ShallowClone();
                        var segment2 = segments0.ShallowClone();
                        var segment3 = segments0.ShallowClone();
                        var segment4 = segments0.ShallowClone();
                        var segment5 = segments0.ShallowClone();
                        segments0.m_backwardForbidden = NetSegment.Flags.AsymBackward | NetSegment.Flags.AsymForward;
                        segments0.m_backwardRequired = NetSegment.Flags.Invert;
                        segments0.m_forwardForbidden = NetSegment.Flags.Invert | NetSegment.Flags.AsymBackward | NetSegment.Flags.AsymForward;
                        segments0.m_forwardRequired = NetSegment.Flags.None;


                        segment1.m_backwardForbidden = NetSegment.Flags.None;
                        segment1.m_backwardRequired = NetSegment.Flags.Bend | NetSegment.Flags.AsymForward;
                        segment1.m_forwardForbidden = NetSegment.Flags.None;
                        segment1.m_forwardRequired = NetSegment.Flags.Bend | NetSegment.Flags.AsymForward;
                        segment1.SetMeshes
                            (@"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_End.obj",
                            @"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Node_LOD.obj");

                     
                        segment2.m_backwardForbidden = NetSegment.Flags.None;
                        segment2.m_backwardRequired = NetSegment.Flags.Bend | NetSegment.Flags.AsymBackward;
                        segment2.m_forwardForbidden = NetSegment.Flags.None;
                        segment2.m_forwardRequired = NetSegment.Flags.Bend | NetSegment.Flags.AsymBackward;
                        segment2.SetMeshes
                            (@"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_End.obj",
                            @"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Node_LOD.obj");
                                               

                        segment3.m_backwardForbidden = NetSegment.Flags.None;
                        segment3.m_backwardRequired = NetSegment.Flags.StopRight;
                        segment3.m_forwardForbidden = NetSegment.Flags.None;
                        segment3.m_forwardRequired = NetSegment.Flags.StopRight;
                        segment3.SetMeshes
                              (@"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Bus.obj",
                              @"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Node_LOD.obj");

                        segment4.m_backwardForbidden = NetSegment.Flags.None;
                        segment4.m_backwardRequired = NetSegment.Flags.StopLeft;
                        segment4.m_forwardForbidden = NetSegment.Flags.None;
                        segment4.m_forwardRequired = NetSegment.Flags.StopLeft;
                        segment4.SetMeshes
                              (@"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Bus.obj",
                              @"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Node_LOD.obj");


                        segment5.m_backwardForbidden = NetSegment.Flags.None;
                        segment5.m_backwardRequired = NetSegment.Flags.Bend | NetSegment.Flags.AsymBackward;
                        segment5.m_forwardForbidden = NetSegment.Flags.None;
                        segment5.m_forwardRequired = NetSegment.Flags.Bend | NetSegment.Flags.AsymBackward;
                        segment5.SetMeshes
                            (@"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_End.obj",
                            @"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Node_LOD.obj");




                        nodes0.SetMeshes
                            (@"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Node.obj",
                            @"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Node_LOD.obj");

                        info.m_segments = new[] { segments0, segment1, segment2, segment3, segment4, segment5 };
                        info.m_nodes = new[] { nodes0 };
                        break;
                    }

               
            }
            return info;
        }


        public static NetInfo Setup40m3mSW1mB2x4mMdnSpecialMesh(this NetInfo info, NetInfoVersion version, SpecailSegments special)
        {
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            var slopeInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_4L_SLOPE);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

            switch (special)
            {
           
                case SpecailSegments.EndNode:
                    {
                        var segments0 = info.m_segments[0];
                        var nodes0 = info.m_nodes[0];

                        segments0
                        .SetFlagsDefault()
                        .SetMeshes
                            (@"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_End.obj",
                            @"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Node_LOD.obj");
           
                        var segment1 = segments0.ShallowClone();
                        var segment2 = segments0.ShallowClone();
                        var segment3 = segments0.ShallowClone();
                        var segment4 = segments0.ShallowClone();

                        segments0.m_backwardForbidden = NetSegment.Flags.AsymForward;
                        segments0.m_backwardRequired = NetSegment.Flags.Invert;
                        segments0.m_forwardForbidden = NetSegment.Flags.Invert | NetSegment.Flags.AsymBackward | NetSegment.Flags.AsymForward;
                        segments0.m_forwardRequired = NetSegment.Flags.None;
                        /// Зеленый
                        segment1.m_backwardForbidden = NetSegment.Flags.None;
                        segment1.m_backwardRequired = NetSegment.Flags.Invert | NetSegment.Flags.Bend | NetSegment.Flags.AsymForward;
                        segment1.m_forwardForbidden = NetSegment.Flags.None;
                        segment1.m_forwardRequired = NetSegment.Flags.Invert | NetSegment.Flags.Bend | NetSegment.Flags.AsymForward;


                       // синий
                        segment2.m_backwardForbidden = NetSegment.Flags.None;
                        segment2.m_backwardRequired = NetSegment.Flags.Invert | NetSegment.Flags.Bend | NetSegment.Flags.AsymBackward;
                        segment2.m_forwardForbidden = NetSegment.Flags.None;
                        segment2.m_forwardRequired = NetSegment.Flags.Invert | NetSegment.Flags.Bend | NetSegment.Flags.AsymBackward;
                        //-------------------------------------------------------------------------------------------------------
                        // голубой
                        segment3.m_backwardForbidden = NetSegment.Flags.Invert;
                        segment3.m_backwardRequired = NetSegment.Flags.Bend | NetSegment.Flags.AsymForward;
                        segment3.m_forwardForbidden = NetSegment.Flags.Invert;
                        segment3.m_forwardRequired = NetSegment.Flags.Bend | NetSegment.Flags.AsymForward;


                        // розовый
                        segment4.m_backwardForbidden = NetSegment.Flags.AsymBackward;
                        segment4.m_backwardRequired = NetSegment.Flags.Invert;
                        segment4.m_forwardForbidden = NetSegment.Flags.Invert | NetSegment.Flags.AsymBackward | NetSegment.Flags.AsymForward;
                        segment4.m_forwardRequired = NetSegment.Flags.None;

                        nodes0.SetMeshes
                            (@"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Node_5L.obj",
                            @"Roads\Common\Meshes\40m\3mSW1mB2x4mMdn\Ground_Node_LOD.obj");

                        info.m_segments = new[] { segments0, segment1, segment2, segment3, segment4};
                        info.m_nodes = new[] { nodes0 };
                        break;
                    }
            }
            return info;
        }
    }
}