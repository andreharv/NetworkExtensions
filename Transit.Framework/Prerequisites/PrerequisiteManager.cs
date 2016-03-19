using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Transit.Framework.Mod;
using UnityEngine;

namespace Transit.Framework.Prerequisites
{
    public class PrerequisiteManager : Singleton<PrerequisiteManager>
    {
        private readonly IDictionary<PrerequisiteType, ICollection<Type>> _installedPrerequisites = new Dictionary<PrerequisiteType, ICollection<Type>>();

        public void InstallPrerequisites(ITransitMod mod)
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

        public void UninstallPrerequisites(ITransitMod mod)
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

        private IEnumerable<IPrerequisiteSetup> GetAllPrerequisites()
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
                        Log.Error("TFW: Crashed-Prerequisites " + t.Name);
                        Log.Error("TFW: " + ex.Message);
                        Log.Error("TFW: " + ex.ToString());
                        return null;
                    }
                })
                .Where(t => t != null)
                .ToArray();
        }

        private void DoInstallation(PrerequisiteType type)
        {
            try
            {
                foreach (var p in GetAllPrerequisites())
                {
                    Log.Info(string.Format("TFW: Installing Prerequisite {0}", p.GetType().Name));
                    p.Install(type);
                }
            }
            catch (Exception ex)
            {
                Log.Error("TFW: Crashed-Prerequisites Installation");
                Log.Error("TFW: " + ex.Message);
                Log.Error("TFW: " + ex.ToString());
            }
        }

        private void DoUninstallation(PrerequisiteType type)
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
                Log.Error("TFW: Crashed-Prerequisites Uninstallation");
                Log.Error("TFW: " + ex.Message);
                Log.Error("TFW: " + ex.ToString());
            }
        }
    }
}
