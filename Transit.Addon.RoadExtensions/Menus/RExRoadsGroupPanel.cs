using ColossalFramework;
using Transit.Framework;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Menus
{
    public class RExRoadsGroupPanel : GeneratedGroupPanel
    {
        [RedirectFrom(typeof(RoadsGroupPanel))]
        protected override int GetCategoryOrder(string name)
        {
            if (isMapEditor)
            {
                switch (name)
                {
                    case RExExtendedMenus.ROADS_TINY:
                        return 0;
                    case "RoadsSmall":
                        return 10;
                    case RExExtendedMenus.ROADS_SMALL_HV:
                        return 20;
                    case "RoadsMedium":
                        return 30;
                    case "RoadsLarge":
                        return 40;
                    case "RoadsHighway":
                        return 50;
                    case "RoadsIntersection":
                        return 60;
                    case "PublicTransportBus":
                        return 70;
                    case "PublicTransportMetro":
                        return 80;
                    case "PublicTransportTrain":
                        return 90;
                    case "PublicTransportShip":
                        return 100;
                    case "PublicTransportPlane":
                        return 110;
                }
                return 2147483647;
            }
            if (isAssetEditor)
            {
                switch (name)
                {
                    case RExExtendedMenus.ROADS_TINY:
                        return 0;
                    case "RoadsSmall":
                        return 10;
                    case RExExtendedMenus.ROADS_SMALL_HV:
                        return 20;
                    case "RoadsMedium":
                        return 30;
                    case "RoadsLarge":
                        return 40;
                    case "RoadsHighway":
                        return 50;
                    case "RoadsIntersection":
                        return 60;
                    case RExExtendedMenus.ROADS_BUSWAYS:
                        return 70;
                    case "PublicTransportTrain":
                        return 80;
                    case RExExtendedMenus.ROADS_PEDESTRIANS:
                        return 90;
                }
                return 2147483647;
            }

            switch (name)
            {
                case RExExtendedMenus.ROADS_TINY:
                    return 0;
                case "RoadsSmall":
                    return 10;
                case RExExtendedMenus.ROADS_SMALL_HV:
                    return 20;
                case "RoadsMedium":
                    return 30;
                case "RoadsLarge":
                    return 40;
                case "RoadsHighway":
                    return 50;
                case "RoadsIntersection":
                    return 60;
                case RExExtendedMenus.ROADS_BUSWAYS:
                    return 70;
                case RExExtendedMenus.ROADS_PEDESTRIANS:
                    return 80;
            }
            return 2147483647;
        }
    }
}
