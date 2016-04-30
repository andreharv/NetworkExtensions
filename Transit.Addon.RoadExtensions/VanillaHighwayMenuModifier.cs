using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions
{
    public class VanillaMenuIconModifier : IModulePart, INetInfoModifier
    {
        public string Name{ get { return "Vanilla Menu Icon Modifier"; } }

        public void ModifyExistingNetInfo()
        {
            #region Highway NetInfos
            var highwayRampInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_RAMP, false);
            if (highwayRampInfo != null)
            {
                highwayRampInfo.m_UIPriority = 5;
                var thumbnails = AssetManager.instance.GetThumbnails(NetInfos.Vanilla.HIGHWAY_RAMP, @"Roads\Highways\HighwayRamp\thumbnails.png");
                highwayRampInfo.m_Atlas = thumbnails;
                highwayRampInfo.m_Thumbnail = thumbnails.name;
            }

            var highway3L = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L, false);
            if (highway3L != null)
            {
                highway3L.m_UIPriority = 30;
                var thumbnails = AssetManager.instance.GetThumbnails(NetInfos.Vanilla.HIGHWAY_3L, @"Roads\Highways\Highway3L\thumbnails.png");
                highway3L.m_Atlas = thumbnails;
                highway3L.m_Thumbnail = thumbnails.name;
                highway3L.ModifyTitle("Three-Lane Highway");
            }

            var highway3LBarrier = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_BARRIER, false);
            if (highway3LBarrier != null)
            {
                highway3LBarrier.m_UIPriority = 35;
                var thumbnails = AssetManager.instance.GetThumbnails(NetInfos.Vanilla.HIGHWAY_3L_BARRIER, @"Roads\Highways\Highway3LBarrier\thumbnails.png");
                highway3LBarrier.m_Atlas = thumbnails;
                highway3LBarrier.m_Thumbnail = thumbnails.name;
                highway3LBarrier.ModifyTitle("Three-Lane Highway with Sound Barrier");
            }
            #endregion

            #region Rail Netinfos
            var rail2L = Prefabs.Find<NetInfo>(NetInfos.Vanilla.TRAINTRACK, false);
            if (rail2L != null)
            {
                rail2L.m_UIPriority = 12;
                var thumbnails = AssetManager.instance.GetThumbnails(NetInfos.Vanilla.TRAINTRACK, @"PublicTransport\Rail\Rail2L\thumbnails.png");
                rail2L.m_Atlas = thumbnails;
                rail2L.m_Thumbnail = thumbnails.name;
                rail2L.ModifyTitle("Two Lane Two Way Rail");
            }
            #endregion
        }
    }
}
