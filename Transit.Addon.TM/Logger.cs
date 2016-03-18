using System;
using UnityEngine;

namespace Transit.Addon.TM
{
    static class Logger
    {
        private const string Prefix = "Traffic++ V2: ";

        public static void LogInfo(string message, params object[] args)
        {
            var msg = Prefix + String.Format(message, args);
            Debug.Log(msg);
        }

        public static void LogWarning(string message, params object[] args)
        {
            var msg = Prefix + String.Format(message, args);
            Debug.LogWarning(msg);
        }

        public static void LogError(string message, params object[] args)
        {
            var msg = Prefix + String.Format(message, args);
            Debug.LogError(msg);
        }
    }
}
