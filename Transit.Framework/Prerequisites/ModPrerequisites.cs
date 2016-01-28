using System;
using System.Collections.Generic;
using System.Linq;
using ICities;
using Transit.Framework.Mod;

namespace Transit.Framework.Prerequisites
{
    public class ModPrerequisites
    {
        private static readonly ICollection<Type> _installedMods = new HashSet<Type>();

        public static void InstallForMod(TransitModBase mod)
        {
            var modType = mod.GetType();

            if (_installedMods.Count == 0)
            {
                DoInstallation();
            }

            if (!_installedMods.Contains(modType))
            {
                _installedMods.Add(modType);
            }
        }

        public static void UninstallForMod(IUserMod mod)
        {
            var modType = mod.GetType();

            if (_installedMods.Contains(modType))
            {
                _installedMods.Remove(modType);
            }

            if (_installedMods.Count == 0)
            {
                DoUninstallation();
            }
        }

        private static IEnumerable<IModPrerequisite> GetAllPrerequisites()
        {
            var prereqType = typeof(IModPrerequisite);
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => prereqType.IsAssignableFrom(t))
                .Select(t =>
                {
                    try
                    {
                        return (IModPrerequisite)Activator.CreateInstance(t);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("TFW: Crashed-Prerequisites " + t.Name);
                        Debug.Log("TFW: " + ex.Message);
                        Debug.Log("TFW: " + ex.ToString());
                        return null;
                    }
                })
                .Where(t => t != null)
                .ToArray();
        }

        private static void DoInstallation()
        {
            foreach (var p in GetAllPrerequisites())
            {
                Debug.Log(string.Format("TFW: Installing Prerequisite {0}", p.GetType().Name));
                p.Install();
            }
        }

        private static void DoUninstallation()
        {
            foreach (var p in GetAllPrerequisites())
            {
                Debug.Log(string.Format("TFW: Uninstalling Prerequisites {0}", p.GetType().Name));
                p.Uninstall();
            }
        }
    }
}
