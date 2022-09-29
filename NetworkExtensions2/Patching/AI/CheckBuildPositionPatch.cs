using HarmonyLib;
using System.Reflection;
using Transit.Framework.ExtensionPoints.AI;

namespace NetworkExtensions2.Patching
{
    [HarmonyPatch(typeof(NetAI), nameof(NetAI.CheckBuildPosition))]
    public class CheckBuildPositionPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref ToolBase.ToolErrors __result)
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
