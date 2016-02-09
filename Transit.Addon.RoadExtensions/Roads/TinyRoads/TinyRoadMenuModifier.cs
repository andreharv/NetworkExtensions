using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;

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
                gravelRoadInfo.SetUICategory(RExExtendedMenus.ROADS_TINY);
                gravelRoadInfo.m_UIPriority = 0;
            }
        }
    }
}
