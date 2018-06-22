using System;
using System.Collections.Generic;
using System.Linq;
using ICities;
using UnityEngine;
using TransitPlus.Framework.Mod;

namespace TransitPlus.Framework.Prerequisites
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
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("TFW: Crashed-Prerequisites looking into assembly " + a.FullName);
                        Debug.Log("TFW: " + ex.Message);
                        Debug.Log("TFW: " + ex.ToString());
                        return new Type[] {};
                    }
                })
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
            try
            {
                foreach (var p in GetAllPrerequisites())
                {
                    Debug.Log(string.Format("TFW: Installing Prerequisite {0}", p.GetType().Name));
                    p.Install();
                }
            }
            catch (Exception ex)
            {
                Debug.Log("TFW: Crashed-Prerequisites Installation");
                Debug.Log("TFW: " + ex.Message);
                Debug.Log("TFW: " + ex.ToString());
            }
        }

        private static void DoUninstallation()
        {
            try
            {
                foreach (var p in GetAllPrerequisites())
                {
                    Debug.Log(string.Format("TFW: Uninstalling Prerequisites {0}", p.GetType().Name));
                    p.Uninstall();
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
