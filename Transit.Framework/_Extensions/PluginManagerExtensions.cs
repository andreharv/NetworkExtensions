using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework.Plugins;
using UnityEngine;

namespace Transit.Framework
{
    public static class PluginManagerExtensions
    {
        public static bool IsPluginInstalled(this PluginManager pluginManager, string pluginId)
        {
            foreach (var plugin in pluginManager.GetPluginsInfo())
            {
                Debug.Log(string.Format("RARARA: plugin name {0}", plugin.name));
                if (plugin.name.Equals(pluginId))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsPluginActive(this PluginManager pluginManager, string pluginId)
        {
            foreach (var plugin in pluginManager.GetPluginsInfo())
            {
                if (plugin.isEnabled)
                {
                    if (plugin.name.Equals(pluginId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
