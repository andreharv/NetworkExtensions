using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup8m1p5mSW1SMesh(this NetInfo info, NetInfoVersion version, LanesLayoutStyle laneStyle = LanesLayoutStyle.Symmetrical)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[0].ShallowClone();
                        var nodes2 = info.m_nodes[0].ShallowClone();
                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Roads\Common\Meshes\8m\1p5mSW1S\Ground.obj",
                             @"Roads\Common\Meshes\8m\1p5mSW1S\Ground_LOD.obj");

                        nodes0
                            .SetMeshes
                            (@"Roads\Common\Meshes\8m\1p5mSW1S\Ground_Node.obj",
                             @"Roads\Common\Meshes\8m\1p5mSW1S\Ground_Node_LOD.obj");

                        nodes1
                            .SetMeshes
                            (@"Roads\Common\Meshes\8m\1p5mSW1S\Ground_Node2.obj",
                             @"Roads\Common\Meshes\8m\1p5mSW1S\Ground_Node_LOD.obj");
                        nodes2
                            .SetMeshes
                            (@"Roads\Common\Meshes\8m\1p5mSW1S\Ground_Node3.obj",
                             @"Roads\Common\Meshes\8m\1p5mSW1S\Ground_Node_LOD.obj");

                        if (laneStyle != LanesLayoutStyle.Symmetrical)
                            RoadHelper.HandleAsymSegmentFlags(segments0);

                        nodes1.m_directConnect = true;
                        nodes1.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart | NetInfo.ConnectGroup.OnewayEnd;
                        nodes2.m_directConnect = true;
                        nodes2.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0, nodes1 };
                        break;
                    }
            }
            return info;
        }
    }
}