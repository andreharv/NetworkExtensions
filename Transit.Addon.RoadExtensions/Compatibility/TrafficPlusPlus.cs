using ColossalFramework;
using ColossalFramework.Plugins;
using Transit.Framework;

namespace Transit.Addon.RoadExtensions.Compatibility
{
    public static class TrafficPlusPlus
    {
        private const string MOD_ID = "417585852";
        public static bool IsPluginActive
        {
            get { return Singleton<PluginManager>.instance.IsPluginActive(MOD_ID); }
        }
    }
}
