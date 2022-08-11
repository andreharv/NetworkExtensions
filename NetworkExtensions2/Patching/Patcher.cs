using HarmonyLib;
using NetworkExtensions;

namespace NetworkExtensions2.Patching
{
    internal class Patcher
    {
        private static Harmony harmony => new Harmony("andreharv.CSL.NetworkExtensions2");
        internal static void PatchAll()
        {
            CheckBuildPositionPatch.Apply(harmony);
            GetLengthSnapPatch.Apply(harmony);
            GetCategoryOrderPatch.Apply(harmony);
            SpawnButtonEntryPatch.Apply(harmony);
        }
        internal static void UnpatchAll()
        {
            harmony.UnpatchAll();
        }
    }
}
