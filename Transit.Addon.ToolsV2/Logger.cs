using System;
using UnityEngine;

namespace Transit.Addon.ToolsV2
{
    static class Logger
    {
        private static readonly string Prefix = "Traffic++ V2: ";
        //private static readonly bool inGameDebug = Environment.OSVersion.Platform != PlatformID.Unix;

        public static void LogInfo(string message, params object[] args)
        {
            var msg = Prefix + String.Format(message, args);
            Debug.Log(msg);
            //if (inGameDebug)
            //    DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, msg);
        }

        public static void LogWarning(string message, params object[] args)
        {
            var msg = Prefix + String.Format(message, args);
            Debug.LogWarning(msg);
            //if (inGameDebug)
            //    DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, msg);
        }

        public static void LogError(string message, params object[] args)
        {
            var msg = Prefix + String.Format(message, args);
            Debug.LogError(msg);
            //if (inGameDebug)
            //    DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, msg);
        }
    }
}
