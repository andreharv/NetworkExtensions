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
                        return 1;
                    case RExExtendedMenus.ROADS_SMALL_HV:
                        return 2;
                    case "RoadsMedium":
                        return 3;
                    case "RoadsLarge":
                        return 4;
                    case "RoadsHighway":
                        return 5;
                    case "RoadsIntersection":
                        return 6;
                    case "PublicTransportBus":
                        return 7;
                    case "PublicTransportMetro":
                        return 8;
                    case "PublicTransportTrain":
                        return 9;
                    case "PublicTransportShip":
                        return 10;
                    case "PublicTransportPlane":
                        return 11;
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
                        return 1;
                    case RExExtendedMenus.ROADS_SMALL_HV:
                        return 2;
                    case "RoadsMedium":
                        return 3;
                    case "RoadsLarge":
                        return 4;
                    case "RoadsHighway":
                        return 5;
                    case "RoadsIntersection":
                        return 6;
                    case RExExtendedMenus.ROADS_BUSWAYS:
                        return 7;
                    case "PublicTransportTrain":
                        return 8;
                    case RExExtendedMenus.ROADS_PEDESTRIANS:
                        return 9;
                }
                return 2147483647;
            }

            switch (name)
            {
                case RExExtendedMenus.ROADS_TINY:
                    return 0;
                case "RoadsSmall":
                    return 1;
                case RExExtendedMenus.ROADS_SMALL_HV:
                    return 2;
                case "RoadsMedium":
                    return 3;
                case "RoadsLarge":
                    return 4;
                case "RoadsHighway":
                    return 5;
                case "RoadsIntersection":
                    return 6;
                case RExExtendedMenus.ROADS_BUSWAYS:
                    return 7;
                case RExExtendedMenus.ROADS_PEDESTRIANS:
                    return 8;
            }
            return 2147483647;
        }
    }
}
