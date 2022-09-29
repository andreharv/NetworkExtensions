using HarmonyLib;
using NetworkExtensions;
using System;
using System.Reflection;
using Transit.Framework.ExtensionPoints.AI;
using UnityEngine;

namespace NetworkExtensions2.Patching
{
    [HarmonyPatch(typeof(RoadAI), nameof(RoadAI.CreateZoneBlocks))]
    internal static class CreateZoneBlocksPatch
    {
        private const float MIN_HALFWIDTH_DEFAULT = 8f;
        [HarmonyPrefix]
        public static bool Prefix(RoadAI __instance, ushort segment, ref NetSegment data)
        {
            try
            {
                if (!Mod.FoundZoningAdjuster && RoadZoneBlocksCreationManager.HasCustomCreator(__instance.m_info.name))
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
