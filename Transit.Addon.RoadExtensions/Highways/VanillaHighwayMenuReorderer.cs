using System;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Highways
{
    public class VanillaHighwayMenuReorderer : IModulePart, INetInfoModifier
    {
        public string Name{ get { return "Vanilla Highway Menu Reorderer"; } }

        public void ModifyExistingNetInfo()
        {
            var highwayRampInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_RAMP, false);
            if (highwayRampInfo != null)
            {
                highwayRampInfo.m_UIPriority = 5;
            }

            var highway3L = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L, false);
            if (highway3L != null)
            {
                highway3L.m_UIPriority = 30;
            }

            var highway3LBarrier = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_BARRIER, false);
            if (highway3LBarrier != null)
            {
                highway3LBarrier.m_UIPriority = 35;
            }
        }
    }
}
