using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Modularity;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Addon
{
    public partial class Mod
    {
        private IEnumerable<IModule> _modules;
        public IEnumerable<IModule> Modules
        {
            get
            {
                if (_modules == null)
                {
                    try
                    {
                        var assetPath = this.GetAssetPath();
                        var moduleType = typeof(IModule);

                        _modules = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .Where(t => !t.IsAbstract && !t.IsInterface)
                            .Where(t => moduleType.IsAssignableFrom(t))
                            .Where(t => t.GetCustomAttributes(typeof(ModuleAttribute), true)
                                         .OfType<ModuleAttribute>()
                                         .Any(a => a.IsAssociatedWith(typeof(Mod))))
                            .Select(t =>
                                {
                                    try
                                    {
                                        var module = (IModule)Activator.CreateInstance(t);
                                        Debug.Log(string.Format("TAM: Loading module {0}", module.Name));

                                        module.AssetPath = assetPath;
                                        module.SaveSettingsNeeded += ModuleSettingsNeedSave;
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

                        _modules = new IModule[] {};
                    }
                }

                return _modules;
            }
        }
    }
}
