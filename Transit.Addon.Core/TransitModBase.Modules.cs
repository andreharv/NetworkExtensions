using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework.Modularity;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Addon.Core
{
    public partial class TransitModBase
    {
        private bool _modulesLoaded;
        private IEnumerable<IModule> _modules = new IModule[] { };
        public IEnumerable<IModule> Modules
        {
            get { return _modules; }
        }

        protected void LoadModulesIfNeeded()
        {
            if (_modulesLoaded)
            {
                return;
            }

            try
            {
                var assetPath = this.GetAssetPath();
                var moduleType = typeof(IModule);
                var modType = this.GetType();

                _modules = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => !t.IsAbstract && !t.IsInterface)
                    .Where(t => moduleType.IsAssignableFrom(t))
                    .Where(t => t.GetCustomAttributes(typeof(ModuleAttribute), true)
                        .OfType<ModuleAttribute>()
                        .Any(a => a.IsAssociatedWith(modType)))
                    .Select(t =>
                    {
                        try
                        {
                            var module = (IModule)Activator.CreateInstance(t);
                            Debug.Log(string.Format("TAM: Loading module {0}", module.Name));

                            module.AssetPath = assetPath;
                            module.SaveSettingsNeeded += SaveSettings;
                            return module;
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("TAM: Crashed-Module " + t.Name);
                            Debug.Log("TAM: " + ex.Message);
                            Debug.Log("TAM: " + ex.ToString());
                            return null;
                        }
                    })
                    .Where(t => t != null)
                    .OrderBy(m => m.Order)
                    .ToArray();
            }
            catch (Exception ex)
            {
                Debug.Log("TAM: Crashed-Modules");
                Debug.Log("TAM: " + ex.Message);
                Debug.Log("TAM: " + ex.ToString());
            }
            finally
            {
                _modulesLoaded = true;
            }
        }
    }
}
