using System;
using System.Collections.Generic;
using System.Linq;
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
                    var moduleType = typeof(IModule);

                    _modules = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes())
                                .Where(t => !t.IsAbstract && !t.IsInterface)
                                .Where(t => moduleType.IsAssignableFrom(t))
                                .Where(t => t.GetCustomAttributes(typeof(ModuleAttribute), true)
                                             .OfType<ModuleAttribute>()
                                             .Any(a => a.Mod == typeof(Mod)))
                                .Select(t =>
                                    {
                                        var module = (IModule)Activator.CreateInstance(t);
                                        Debug.Log(string.Format("TAM: Loading module {0}", module.Name));

                                        module.SaveSettingsNeeded += ModuleSettingsNeedSave;
                                        return module;
                                    })
                                .OrderBy(m => m.Name)
                                .ToArray();
                }

                return _modules;
            }
        }
    }
}
