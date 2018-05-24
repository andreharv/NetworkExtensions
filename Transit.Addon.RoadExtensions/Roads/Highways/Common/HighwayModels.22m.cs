using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Network;
using System.Collections.Generic;
using System.Linq;

namespace Transit.Addon.RoadExtensions.Roads.Highways
{
    public static partial class HighwayModels
    {
        public static void Setup22mMesh(this NetInfo info, NetInfoVersion version, LanesLayoutStyle lanesLayoutStyle = LanesLayoutStyle.Symmetrical)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;
            var defaultLODMaterial = highwayInfo.m_nodes[0].m_lodMaterial;
            if (version== NetInfoVersion.Ground)
            {
                if (lanesLayoutStyle != LanesLayoutStyle.Symmetrical)
                {
                    var segment0 = info.m_segments[0].ShallowClone();
                    RoadHelper.HandleAsymSegmentFlags(segment0);
                    info.m_segments = new[] { segment0 };
                }
                
            }
            if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
            {
                if (lanesLayoutStyle != LanesLayoutStyle.Symmetrical)
                {
                    var segment0 = info.m_segments[0].ShallowClone();
                    RoadHelper.HandleAsymSegmentFlags(segment0);
                    var segments = info.m_segments.ToList();
                    segments[0] = segment0;
                    info.m_segments = segments.ToArray();
                }
            }
            if (version == NetInfoVersion.Slope)
            {
                var segments0 = info.m_segments[0].ShallowClone();
                var segments1 = info.m_segments[1].ShallowClone();
                var segments2 = info.m_segments[1].ShallowClone();
                var nodes0 = info.m_nodes[0].ShallowClone();
                var nodes1 = info.m_nodes[1].ShallowClone();
                var nodes2 = info.m_nodes[0].ShallowClone();
                //var nodes1 = info.m_nodes[1].ShallowClone();
                segments1
                    .SetMeshes
                   (@"Roads\Highways\Common\Meshes\22m\Slope.obj");

                segments2
                    .SetMeshes
                   (@"Roads\Highways\Common\Meshes\22m\Slope_Cover.obj",
                    @"Roads\Common\Meshes\Blank.obj")
                    .SetConsistentUVs();

                nodes2
                    .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                    .SetMeshes
                    (@"Roads\Highways\Common\Meshes\22m\Slope_U_Node.obj",
                    @"Roads\Highways\Common\Meshes\22m\Slope_U_Node_LOD.obj");
                //nodes1
                //    .SetFlags(NetNode.Flags.UndergroundTransition, NetNode.Flags.None)
                //    .SetMeshes
                //    (@"Roads\Highways\Common\Meshes\22m\Slope_U_Trans.obj",
                //    @"Roads\Highways\Common\Meshes\22m\Slope_U_Trans_LOD.obj");
                if (lanesLayoutStyle != LanesLayoutStyle.Symmetrical)
                {
                    RoadHelper.HandleAsymSegmentFlags(segments1);
                }
                nodes2.m_material = defaultMaterial;
                nodes2.m_lodMaterial = defaultLODMaterial;
                //nodes1.m_material = defaultMaterial;
                //nodes1.m_lodMaterial = defaultLODMaterial;

                info.m_segments = new[] { segments0, segments1, segments2 };
                info.m_nodes = new[] { nodes0, nodes1, nodes2 };
            }
            else if (version == NetInfoVersion.Tunnel)
            {
                var segments0 = info.m_segments[0].ShallowClone();
                var segments1 = info.m_segments[0].ShallowClone();
                var nodes0 = info.m_nodes[0].ShallowClone();
                var nodes1 = info.m_nodes[0].ShallowClone();
                var nodes2 = info.m_nodes[0].ShallowClone();

                nodes1.m_flagsForbidden = NetNode.Flags.None;
                nodes1.m_flagsRequired = NetNode.Flags.None;

                segments1.SetMeshes
                    (@"Roads\Highways\Common\Meshes\22m\Tunnel.obj",
                    @"Roads\Highways\Common\Meshes\22m\Tunnel_LOD.obj");

                nodes1.SetMeshes
                    (@"Roads\Highways\Common\Meshes\22m\Tunnel_Node.obj",
                     @"Roads\Highways\Common\Meshes\22m\Tunnel_Node_LOD.obj");

                //nodes2.SetMeshes
                //    (@"Roads\Highways\Common\Meshes\22m\Tunnel_Trans.obj",
                //     @"Roads\Highways\Common\Meshes\22m\Tunnel_Trans_LOD.obj");

                if (lanesLayoutStyle != LanesLayoutStyle.Symmetrical)
                {
                    RoadHelper.HandleAsymSegmentFlags(segments1);
                }

                segments1.m_material = defaultMaterial;
                segments1.m_lodMaterial = defaultLODMaterial;
                nodes1.m_material = defaultMaterial;
                nodes1.m_lodMaterial = defaultLODMaterial;

                info.m_segments = new[] { segments0, segments1 };
                info.m_nodes = new[] { nodes0, nodes1, nodes2 };
            }
        }
    }
}
