using ColossalFramework;
using ColossalFramework.Plugins;
using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Compatibility
{
    public static class ModernLightingPack
    {
        public const string MOD_ID = "609644643";
        public static bool IsPluginActive
        {
            get { return Singleton<PluginManager>.instance.IsPluginActive(MOD_ID); }
        }
    }
}
