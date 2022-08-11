using HarmonyLib;
using System;
using System.Reflection;
using Transit.Framework.ExtensionPoints.AI;
using UnityEngine;

namespace NetworkExtensions2.Patching
{
    [HarmonyPatch(typeof(RoadAI))]
    [HarmonyPatch(nameof(RoadAI.CreateZoneBlocks))]
    internal static class CreateZoneBlocksPatch
    {
        public static void Apply(Harmony harmony)
        {
            var prefix = typeof(CreateZoneBlocksPatch).GetMethod(nameof(CreateZoneBlocksPatch.Prefix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(OriginalMethod, new HarmonyMethod(prefix));
        }
        public static void Revert(Harmony harmony)
        {
            harmony.Unpatch(OriginalMethod, HarmonyPatchType.Prefix);
        }

        static MethodInfo OriginalMethod => typeof(RoadAI).GetMethod("CreateZoneBlocks");

        private const float MIN_HALFWIDTH_DEFAULT = 8f;

        static bool Prefix(RoadAI __instance, ushort segment, ref NetSegment data)
        {
            try
            {
                if (RoadZoneBlocksCreationManager.HasCustomCreator(__instance.m_info.name))
                {
                    RoadZoneBlocksCreationManager
                        .GetCustomCreator(__instance.m_info.name)
                        .CreateZoneBlocks(__instance.m_info, segment, ref data);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("TAM: Crashed-CreateZoneBlocks");
                Debug.Log("TAM: " + ex.Message);
                Debug.Log("TAM: " + ex.ToString());
            }

            return true;
        }
    }
}
