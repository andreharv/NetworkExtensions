using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace Transit.Framework.Hooks
{
    public class HooksHandler
    {
        // TODO: Maybe apply Dependency Inversion with TransitModBase

        private static readonly ICollection<Type> _installedMods = new HashSet<Type>();

        public static void InstallHooksForMod(IUserMod mod)
        {
            var modType = mod.GetType();

            if (_installedMods.Count == 0)
            {
                try
                {
                    Redirector.PerformRedirections();
                }
                catch (Exception ex)
                {
                    Debug.Log("TFW: Crashed-HooksInstallation");
                    Debug.Log("TFW: " + ex.Message);
                    Debug.Log("TFW: " + ex.ToString());
                }
            }

            if (!_installedMods.Contains(modType))
            {
                _installedMods.Add(modType);
            }
        }

        public static void UnInstallHooksForMod(IUserMod mod)
        {
            var modType = mod.GetType();

            if (_installedMods.Contains(modType))
            {
                _installedMods.Remove(modType);
            }

            if (_installedMods.Count == 0)
            {
                try
                {
                    Redirector.RevertRedirections();
                }
                catch (Exception ex)
                {
                    Debug.Log("TFW: Crashed-HooksUnInstallation");
                    Debug.Log("TFW: " + ex.Message);
                    Debug.Log("TFW: " + ex.ToString());
                }
            }
        }
    }
}
