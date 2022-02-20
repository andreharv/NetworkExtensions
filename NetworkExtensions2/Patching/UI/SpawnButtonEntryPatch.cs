using ColossalFramework.UI;
using HarmonyLib;
using System;
using System.Reflection;
using Transit.Framework;

namespace NetworkExtensions2.Patching
{
    [HarmonyPatch(typeof(GeneratedGroupPanel))]
    [HarmonyPatch("SpawnButtonEntry")]
    public class SpawnButtonEntryPatch
    {
        public static void Apply(Harmony harmony)
        {
            var postfix = typeof(SpawnButtonEntryPatch).GetMethod(nameof(SpawnButtonEntryPatch.Postfix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(OriginalMethod, null, new HarmonyMethod(postfix));
        }
        public static void Revert(Harmony harmony)
        {
            harmony.Unpatch(OriginalMethod, HarmonyPatchType.Postfix);
        }

        static MethodInfo OriginalMethod => typeof(GeneratedGroupPanel).GetMethod("SpawnButtonEntry", BindingFlags.NonPublic | BindingFlags.Instance, null, 
            new Type[] { typeof(UITabstrip), typeof(string), typeof(string), typeof(bool), typeof(string), typeof(string), typeof(string), typeof(bool), typeof(bool) }, null);

        static void Postfix(ref UIButton __result, string category)
        {
            var customAtlas = AtlasManager.instance.GetAtlas(category);
            if (customAtlas != null)
            {
                __result.atlas = customAtlas;
            }
        }
    }
}
