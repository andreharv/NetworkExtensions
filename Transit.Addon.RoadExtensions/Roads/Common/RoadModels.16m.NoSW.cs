using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadModels
    {
        public static NetInfo Setup16mNoSWMesh(this NetInfo info, NetInfoVersion version)
        {
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var highwaySlopeInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_SLOPE);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;
            var hasBikeLane = (info.m_connectGroup & (NetInfo.ConnectGroup)16) != 0;
            switch (version)
            {
                case NetInfoVersion.Ground:
                    var segments0 = info.m_segments[0];
                    var nodes0 = info.m_nodes[0].ShallowClone();
                    var nodes1 = info.m_nodes[0].ShallowClone();
                    var nodes2 = info.m_nodes[0].ShallowClone();

                    nodes0.m_flagsRequired = NetNode.Flags.None;
                    nodes0.m_flagsForbidden = NetNode.Flags.Transition;

                    nodes1.m_flagsRequired = NetNode.Flags.Transition;
                    nodes1.m_flagsForbidden = NetNode.Flags.None;

                    nodes2.m_connectGroup = (NetInfo.ConnectGroup)16;
                    nodes2.m_flagsRequired = NetNode.Flags.None;
                    nodes2.m_flagsForbidden = NetNode.Flags.Transition;
                    nodes2.m_emptyTransparent = true;
                    nodes2.m_directConnect = true;
                    nodes2.m_requireSurfaceMaps = true;

                    segments0
                        .SetFlagsDefault()
                        .SetMeshes(
                        @"Roads\Common\Meshes\16m\NoSW\Ground.obj");

                    nodes0
                        .SetMeshes(
                        @"Roads\Common\Meshes\16m\NoSW\Ground_Node.obj");

                    nodes1
                        .SetMeshes(
                        @"Roads\Common\Meshes\16m\NoSW\Ground_Trans.obj");

                    nodes2
                        .SetMeshes(
                        @"Roads\Common\Meshes\16m\NoSW\Ground_Node_Bike.obj");



                    info.m_segments = new[] { segments0 };
                    info.m_nodes = new[] { nodes0, nodes1, nodes2 };
                    break;
            }
            return info;
        }
    }
}