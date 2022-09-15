using System;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Plugins;
using Transit.Framework;
using Transit.Framework.Mod;

namespace NetworkExtensions
{
    public sealed partial class Mod : TransitModBase
    {
        public override string Name
        {
            get { return "Network Extensions 2"; }
        }

        public override string Description
        {
            get { return "A continuation of the successful Network Extensions Project"; }
        }

        public override string Version
        {
            get { return "1.0.0"; }
        }

        private const string NEXT_2_ID = "812125426";
        private bool? _isNEXT2Installed;

        public bool IsNEXT2Installed
        {
            get
            {
                if (_isNEXT2Installed == null)
                {
                    _isNEXT2Installed = Singleton<PluginManager>.instance.IsPluginInstalled(NEXT_2_ID);
                }

                return _isNEXT2Installed.Value;
            }
        }
        public static bool FoundZoningAdjuster => ConflictingMods().HasFlag(ConflictingModFlag.ZoningAdjuster);

        private static bool m_ConflictingModNeedPerformSearch = true;
        private static ConflictingModFlag m_ConflictingMods = ConflictingModFlag.None;
        public static ConflictingModFlag ConflictingMods()
        {
            if (m_ConflictingMods == ConflictingModFlag.None && m_ConflictingModNeedPerformSearch)
            {
                m_ConflictingModNeedPerformSearch = false;
                foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
                {
                    UnityEngine.Debug.Log("Plugin Namex: " + plugin.name);
                    foreach (Assembly assembly in plugin.GetAssemblies())
                    {
                        UnityEngine.Debug.Log("Assembly Namex: " + assembly.GetName().Name);
                        switch (assembly.GetName().Name)
                        {
                            case "ZoningAdjuster":
                                m_ConflictingMods |= ConflictingModFlag.ZoningAdjuster;
                                break;
                        }
                    }
                }
            }
            return m_ConflictingMods;
        }

        [Flags]
        public enum ConflictingModFlag : short
        {
            None = 0,
            ZoningAdjuster = 1
        }
    }
}
