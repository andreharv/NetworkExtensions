using ColossalFramework;

namespace Transit.Addon.RoadExtensions.Menus
{
    public class RExRoadsGroupPanel : GeneratedGroupPanel
    {
        protected override int GetCategoryOrder(string name)
        {
            if (isMapEditor)
            {
                switch (name)
                {
                    case "RoadsSmall":
                        return 0;
                    case AdditionnalMenus.ROADS_SMALL_HV:
                        return 1;
                    case "RoadsMedium":
                        return 2;
                    case "RoadsLarge":
                        return 3;
                    case "RoadsHighway":
                        return 4;
                    case "RoadsIntersection":
                        return 5;
                    case "PublicTransportBus":
                        return 6;
                    case "PublicTransportMetro":
                        return 7;
                    case "PublicTransportTrain":
                        return 8;
                    case "PublicTransportShip":
                        return 9;
                    case "PublicTransportPlane":
                        return 10;
                }
                return 2147483647;
            }
            if (isAssetEditor)
            {
                switch (name)
                {
                    case "RoadsSmall":
                        return 0;
                    case AdditionnalMenus.ROADS_SMALL_HV:
                        return 1;
                    case "RoadsMedium":
                        return 2;
                    case "RoadsLarge":
                        return 3;
                    case "RoadsHighway":
                        return 4;
                    case AdditionnalMenus.ROADS_BUSWAYS:
                        return 5;
                    case "PublicTransportTrain":
                        return 6;
                    case "RoadsIntersection":
                        return 7;
                }
                return 2147483647;
            }

            switch (name)
            {
                case "RoadsSmall":
                    return 0;
                case AdditionnalMenus.ROADS_SMALL_HV:
                    return 1;
                case "RoadsMedium":
                    return 2;
                case "RoadsLarge":
                    return 3;
                case "RoadsHighway":
                    return 4;
                case AdditionnalMenus.ROADS_BUSWAYS:
                    return 5;
                case "RoadsIntersection":
                    return 6;
            }
            return 2147483647;
        }
    }
}
