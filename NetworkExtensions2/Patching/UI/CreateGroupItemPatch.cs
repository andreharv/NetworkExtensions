using ColossalFramework;
using ColossalFramework.UI;
using HarmonyLib;
using System;
using System.Reflection;
using Transit.Framework;
using UnityEngine;

namespace NetworkExtensions2.Patching
{
    [HarmonyPatch(typeof(GeneratedGroupPanel), "CreateGroupItem")]
    internal class CreateGroupItemPatch
    {
        private static MethodInfo m_spawnButtonEntry = typeof(GeneratedGroupPanel).GetMethod("SpawnButtonEntry", BindingFlags.Instance | BindingFlags.NonPublic, null , new Type[] { typeof(UITabstrip), typeof(string), typeof(string), typeof(bool), typeof(string), typeof(string), typeof(string), typeof(bool), typeof(bool) }, null);
        [HarmonyPrefix]
        public static bool Prefix(GeneratedGroupPanel __instance, GeneratedGroupPanel.GroupInfo info, string localeID, UITabstrip ___m_Strip)
        {
            var argName = info.subPanelType.IsNullOrWhiteSpace() ? __instance.service.Name() : info.subPanelType;
            var category = info.name;
            var button = (UIButton)m_spawnButtonEntry.Invoke(__instance, new object[] { ___m_Strip, argName, category, false, localeID, info.unlockText, "SubBar", info.isUnlocked, false });
            var customAtlas = AtlasManager.instance.GetAtlas(category);
            if (customAtlas != null)
            {
                button.atlas = customAtlas;
            }
            return false;
        }
    }
}
