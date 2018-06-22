using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework.Modularity;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace TransitPlus.Framework.Mod
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
                var moduleType = typeof(IModule);
                var modType = this.GetType();

                _modules = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a =>
                    {
                        try
                        {
                            return a.GetTypes();
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("TFW: LoadModulesIfNeeded looking into assembly " + a.FullName);
                            Debug.Log("TFW: " + ex.Message);
                            Debug.Log("TFW: " + ex.ToString());
                            return new Type[] { };
                        }
                    })
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
                            Debug.Log(string.Format("TFW: Loading module {0}", module.Name));

                            module.AssetPath = AssetPath;
                            module.SaveSettingsNeeded += SaveSettings;
                            return module;
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("TFW: Crashed-Module " + t.Name);
                            Debug.Log("TFW: " + ex.Message);
                            Debug.Log("TFW: " + ex.ToString());
                            return null;
                        }
                    })
                    .Where(t => t != null)
                    .OrderBy(m => m.Order)
                    .ToArray();
                foreach (var m in _modules)
                {
                    Debug.Log(m.Name);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("TFW: Crashed-Modules");
                Debug.Log("TFW: " + ex.Message);
                Debug.Log("TFW: " + ex.ToString());
            }
            finally
            {
                _modulesLoaded = true;
            }
        }
    }
}
