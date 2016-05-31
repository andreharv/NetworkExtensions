using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;

namespace Transit.Framework.Hooks.UI
{
    public class RoadsGroupPanelHook : GeneratedGroupPanel
    {
        [RedirectFrom(typeof(RoadsGroupPanel), (ulong)PrerequisiteType.UI)]
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
