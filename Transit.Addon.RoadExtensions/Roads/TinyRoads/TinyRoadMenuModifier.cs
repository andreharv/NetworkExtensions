using Transit.Addon.RoadExtensions.UI.Toolbar.Roads;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.TinyRoads
{
    public class TinyRoadMenuModifier : IModulePart, INetInfoModifier
    {
        public string Name{ get { return "Tiny Road Menu Modifier"; } }

        public void ModifyExistingNetInfo()
        {
            var gravelRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L_GRAVEL, false);
            if (gravelRoadInfo != null)
            {
                gravelRoadInfo.SetUICategory(TinyRoadsCategoryBuilder.NAME);
                gravelRoadInfo.m_UIPriority = 0;
            }
        }
    }
}
