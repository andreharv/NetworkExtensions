using HarmonyLib;
using System.Linq;
using System.Reflection;
using Transit.Framework.ExtensionPoints.AI;

namespace NetworkExtensions2.Patching
{
    [HarmonyPatch(typeof(NetAI))]
    [HarmonyPatch(nameof(NetAI.CheckBuildPosition))]
    public class CheckBuildPositionPatch
    {
        public static void Apply(Harmony harmony)
        {
            var postfix = typeof(CheckBuildPositionPatch).GetMethod(nameof(CheckBuildPositionPatch.Postfix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(OriginalMethod, null, new HarmonyMethod(postfix));
        }
        public static void Revert(Harmony harmony)
        {
            harmony.Unpatch(OriginalMethod, HarmonyPatchType.Postfix);
        }

        static MethodInfo OriginalMethod => typeof(NetAI).GetMethod("CheckBuildPosition");

        static void Postfix(ref ToolBase.ToolErrors __result)
        {
            if ((__result & ToolBase.ToolErrors.CannotUpgrade) == ToolBase.ToolErrors.CannotUpgrade)
            {
                if (ZoneBlocksOffset.Mode != ZoneBlocksOffsetMode.Default)
                {
                    __result &= ~ToolBase.ToolErrors.CannotUpgrade;
                }
            }
        }
    }
}
