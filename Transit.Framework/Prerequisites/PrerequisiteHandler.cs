using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Transit.Framework.Mod;
using UnityEngine;

namespace Transit.Framework.Prerequisites
{
    public static class PrerequisiteHandler
    {
        private static readonly IDictionary<PrerequisiteType, ICollection<Type>> _installedPrerequisites = new Dictionary<PrerequisiteType, ICollection<Type>>();

        public static void InstallPrerequisites(this ITransitMod mod)
        {
            var modType = mod.GetType();

            foreach (var requirement in mod.Requirements.GetFlags())
            {
                if (!_installedPrerequisites.ContainsKey(requirement))
                {
                    DoInstallation(requirement);
                    _installedPrerequisites[requirement] = new HashSet<Type>();
                }

                _installedPrerequisites[requirement].Add(modType);
            }
        }

        public static void UninstallPrerequisites(this ITransitMod mod)
        {
            var modType = mod.GetType();

            foreach (var requirement in mod.Requirements.GetFlags())
            {
                if (_installedPrerequisites.ContainsKey(requirement))
                {
                    if (_installedPrerequisites[requirement].Contains(modType))
                    {
                        _installedPrerequisites[requirement].Remove(modType);

                        if (!_installedPrerequisites[requirement].Any())
                        {
                            DoUninstallation(requirement);
                            _installedPrerequisites.Remove(requirement);
                        }
                    }
                }
            }
        }

        private static IEnumerable<IPrerequisiteSetup> GetAllPrerequisites()
        {
            return Extensibility
                .GetImplementations<IPrerequisiteSetup>()
                .Select(t =>
                {
                    try
                    {
                        return (IPrerequisiteSetup)Activator.CreateInstance(t);
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

        private static void DoInstallation(PrerequisiteType type)
        {
            try
            {
                foreach (var p in GetAllPrerequisites())
                {
                    Debug.Log(string.Format("TFW: Installing Prerequisite {0}", p.GetType().Name));
                    p.Install(type);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("TFW: Crashed-Prerequisites Installation");
                Debug.Log("TFW: " + ex.Message);
                Debug.Log("TFW: " + ex.ToString());
            }
        }

        private static void DoUninstallation(PrerequisiteType type)
        {
            try
            {
                foreach (var p in GetAllPrerequisites())
                {
                    p.Uninstall(type);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("TFW: Crashed-Prerequisites Uninstallation");
                Debug.Log("TFW: " + ex.Message);
                Debug.Log("TFW: " + ex.ToString());
            }
        }
    }
}
