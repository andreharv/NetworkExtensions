using HarmonyLib;
using System.Reflection;
using Transit.Framework.ExtensionPoints.UI;

namespace NetworkExtensions2.Patching
{
    [HarmonyPatch(typeof(RoadsGroupPanel), "GetCategoryOrder")]
    internal static class GetCategoryOrderPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref int __result, string name)
        {
            int? order = RoadCategoryOrderManager.GetOrder(name);

            if (order != null)
            {
                __result = order.Value;
                return false;
            }

            if (ToolsModifierControl.isMapEditor)
            {
                switch (name)
                {
                    case "RoadsSmall":
                        __result = 10;
                        return false;
                    case "RoadsMedium":
                        __result = 30;
                        return false;
                    case "RoadsLarge":
                        __result = 40;
                        return false;
                    case "RoadsHighway":
                        __result = 50;
                        return false;
                    case "RoadsIntersection":
                        __result = 60;
                        return false;
                    case "PublicTransportBus":
                        __result = 70;
                        return false;
                    case "PublicTransportMetro":
                        __result = 80;
                        return false;
                    case "PublicTransportTrain":
                        __result = 90;
                        return false;
                    case "PublicTransportShip":
                        __result = 100;
                        return false;
                    case "PublicTransportPlane":
                        __result = 110;
                        return false;
                    case "PublicTransportTaxi":
                        __result = 120;
                        return false;
                    case "PublicTransportTram":
                        __result = 130;
                        return false;
                    case "PublicTransportMonorail":
                        __result = 140;
                        return false;
                    case "PublicTransportCableCar":
                        __result = 150;
                        return false;
                }
                __result = 2147483647;
                return false;
            }
            if (ToolsModifierControl.isAssetEditor)
            {
                switch (name)
                {
                    case "RoadsSmall":
                        __result = 10;
                        return false;
                    case "RoadsMedium":
                        __result = 30;
                        return false;
                    case "RoadsLarge":
                        __result = 40;
                        return false;
                    case "RoadsHighway":
                        __result = 50;
                        return false;
                    case "RoadsIntersection":
                        __result = 60;
                        return false;
                    case "PublicTransportTrain":
                        __result = 70;
                        return false;
                }
                __result = 2147483647;
                return false;
            }

            switch (name)
            {
                case "RoadsSmall":
                    __result = 10;
                    return false;
                case "RoadsMedium":
                    __result = 30;
                    return false;
                case "RoadsLarge":
                    __result = 40;
                    return false;
                case "RoadsHighway":
                    __result = 50;
                    return false;
                case "RoadsIntersection":
                    __result = 60;
                    return false;
            }
            __result = 2147483647;
            return false;
        }
    }
}
