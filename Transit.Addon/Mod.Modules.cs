﻿using System;
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
        private static IEnumerable<IModule> s_modules;
        public static IEnumerable<IModule> Modules
        {
            get
            {
                if (s_modules == null)
                {
                    var moduleType = typeof(IModule);

                    s_modules = AppDomain
                        .CurrentDomain
                        .GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => !t.IsAbstract && !t.IsInterface)
                        .Where(moduleType.IsAssignableFrom)
                        .Where(t => t.GetCustomAttributes(typeof(ModuleAttribute), true)
                                     .OfType<ModuleAttribute>()
                                     .Any(a => a.Mod == typeof(Mod)))
                        .Select(t =>
                            {
                                var module = (IModule)Activator.CreateInstance(t);
                                Debug.Log(string.Format("TAM: Loading module {0}", module.Name));
                                return module;
                            })
                        .ToArray();
                }

                return s_modules;
            }
        }
    }
}
