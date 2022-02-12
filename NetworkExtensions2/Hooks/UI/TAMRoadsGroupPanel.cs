using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework.Redirection;

namespace Transit.Framework.Hooks.UI
{
    public class TAMRoadsGroupPanel : GeneratedGroupPanel
    {
        [RedirectFrom(typeof(RoadsGroupPanel))]
        protected override int GetCategoryOrder(string name)
        {
            int? order = RoadCategoryOrderManager.GetOrder(name);

            if (order != null)
            {
                return order.Value;
            }

            if (isMapEditor)
            {
                switch (name)
                {
                    case "RoadsSmall":
                        return 10;
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
                    case "PublicTransportTaxi":
                        return 120;
                    case "PublicTransportTram":
                        return 130;
                    case "PublicTransportMonorail":
                        return 140;
                    case "PublicTransportCableCar":
                        return 150;
                }
                return 2147483647;
            }
            if (isAssetEditor)
            {
                switch (name)
                {
                    case "RoadsSmall":
                        return 10;
                    case "RoadsMedium":
                        return 30;
                    case "RoadsLarge":
                        return 40;
                    case "RoadsHighway":
                        return 50;
                    case "RoadsIntersection":
                        return 60;
                    case "PublicTransportTrain":
                        return 70;
                }
                return 2147483647;
            }

            switch (name)
            {
                case "RoadsSmall":
                    return 10;
                case "RoadsMedium":
                    return 30;
                case "RoadsLarge":
                    return 40;
                case "RoadsHighway":
                    return 50;
                case "RoadsIntersection":
                    return 60;
            }
            return 2147483647;
        }
    }
}
