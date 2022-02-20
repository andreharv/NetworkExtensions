using HarmonyLib;
using System.Reflection;
using Transit.Framework.ExtensionPoints.AI;

namespace NetworkExtensions2.Patching
{
    [HarmonyPatch(typeof(NetAI))]
    [HarmonyPatch(nameof(NetAI.GetLengthSnap))]
    internal static class GetLengthSnapPatch
    {
        public static void Apply(Harmony harmony)
        {
            var prefix = typeof(GetLengthSnapPatch).GetMethod(nameof(GetLengthSnapPatch.Prefix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(OriginalMethod, new HarmonyMethod(prefix));
        }
        public static void Revert(Harmony harmony)
        {
            harmony.Unpatch(OriginalMethod, HarmonyPatchType.Prefix);
        }

        static MethodInfo OriginalMethod => typeof(NetAI).GetMethod(nameof(NetAI.GetLengthSnap));
        static bool Prefix(NetAI __instance, ref float __result)
        {
            if (!(__instance is RoadAI))
            {
                return true;
            }
            if (RoadSnappingModeManager.HasCustomSnapping(__instance.m_info.name))
            {
                __result = RoadSnappingModeManager
                    .GetCustomSnapping(__instance.m_info.name)
                    .GetLengthSnap();
                return false;
            }
            else if (!((RoadAI)__instance).m_enableZoning)
            {
                __result = 0f;
                return false;
            }

            return true;
        }
    }
}
