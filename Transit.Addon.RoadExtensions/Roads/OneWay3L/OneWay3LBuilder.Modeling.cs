using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Roads.OneWay3L
{
    public partial class OneWay3LBuilder
    {
        private static void SetupModels(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var defaultMaterial = highwayInfo.m_nodes[0].m_material;

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
                                (@"Roads\aRoadTemplates\Meshes\16m\3mSW\Elevated.obj",
                                @"Roads\aRoadTemplates\Meshes\16m\3mSW\Elevated_LOD.obj");

                        nodes0.SetMeshes
                            (@"Roads\aRoadTemplates\Meshes\16m\3mSW\Elevated_Node.obj",
                            @"Roads\aRoadTemplates\Meshes\16m\3mSW\Elevated_Node_LOD.obj");

                        info.m_segments = new[] { segments0 };
                        info.m_nodes = new[] { nodes0 };
                        break;
                    }
            }
        }
    }
}