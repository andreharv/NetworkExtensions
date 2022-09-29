using System.Reflection;
using HarmonyLib;

namespace NetworkExtensions2.Patching
{
    public static class Patcher
    {
        private const string HarmonyId = "boformer.Harmony2Example";

        private static bool patched = false;

        public static void PatchAll()
        {
            if (patched) return;

            UnityEngine.Debug.Log("Harmony 2 NExt2: Patching...");

            patched = true;

            // Apply your patches here!
            // Harmony.DEBUG = true;
            var harmony = new Harmony("andreharv.CSL.NetworkExtensions2");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UnpatchAll()
        {
            if (!patched) return;

            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);

            patched = false;

            UnityEngine.Debug.Log("Harmony 2 NExt2: Reverted...");
        }
    }
}
