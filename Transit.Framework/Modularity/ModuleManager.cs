using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using Transit.Framework.Mod;
using UnityEngine;

namespace Transit.Framework.Modularity
{
    public class ModuleManager : Singleton<ModuleManager>
    {
        private readonly IDictionary<ITransitMod, IEnumerable<Type>> _modModuleTypes = new Dictionary<ITransitMod, IEnumerable<Type>>();
        private readonly IDictionary<IModule, ICollection<ITransitMod>> _activeModules = new Dictionary<IModule, ICollection<ITransitMod>>();
        private readonly IDictionary<Type, IModule> _activeModuleTypes = new Dictionary<Type, IModule>();

        private bool IsModOwnerOfModule(ITransitMod mod, IModule module)
        {
            if (!_activeModules.ContainsKey(module))
            {
                return false;
            }

            var owners = _activeModules[module];

            if (!owners.Contains(mod))
            {
                return false;
            }

            if (mod.Type == TransitModType.Master)
            {
                return true;
            }

            if (owners.Except(mod).Any(m => m.Type == TransitModType.Master))
            {
                return false;
            }

            if (!owners.Any())
            {
                return false;
            }

            return owners.First() == mod;
        }

        private static IModule CreateModule(Type moduleType)
        {
            try
            {
                var module = (IModule)Activator.CreateInstance(moduleType);
                Debug.Log(string.Format("TFW: Loading module {0}", module.Name));
                return module;
            }
            catch (Exception ex)
            {
                Debug.Log("TFW: Crashed-Module " + moduleType.Name);
                Debug.Log("TFW: " + ex.Message);
                Debug.Log("TFW: " + ex.ToString());
                return null;
            }
        }

        public void RegisterModules(ITransitMod mod)
        {
            var modType = mod.GetType();

            var moduleTypes = Extensibility
                .GetImplementations<IModule>()
                .Where(t => t.GetCustomAttributes(typeof (ModuleAttribute), true)
                             .OfType<ModuleAttribute>()
                             .Any(a => a.IsAssociatedWith(modType)))
                .ToArray();

            _modModuleTypes[mod] = moduleTypes;
        }

        public IEnumerable<IModule> GetModules(ITransitMod mod)
        {
            if (!_modModuleTypes.ContainsKey(mod))
            {
                yield break;
            }

            foreach (var moduleType in _modModuleTypes[mod])
            {
                var module = GetOrCreateModule(mod, moduleType, true);

                if (module != null)
                {
                    yield return module;
                }
            }
        }

        public IEnumerable<IModule> GetOrCreateModules(ITransitMod mod)
        {
            if (!_modModuleTypes.ContainsKey(mod))
            {
                yield break;
            }

            foreach (var moduleType in _modModuleTypes[mod])
            {
                var module = GetOrCreateModule(mod, moduleType, false);

                if (module != null)
                {
                    yield return module;
                }
            }
        }

        private IModule GetOrCreateModule(ITransitMod mod, Type moduleType, bool skipCreationIfNotFound)
        {
            if (!typeof (IModule).IsAssignableFrom(moduleType))
            {
                throw new Exception(string.Format("Module {0} is not supported by the ModuleManager", moduleType));
            }

            if (!_activeModuleTypes.ContainsKey(moduleType))
            {
                if (skipCreationIfNotFound)
                {
                    return null;
                }

                var newModule = CreateModule(moduleType);
                if (newModule == null)
                {
                    return null;
                }

                _activeModuleTypes.Add(moduleType, newModule);
            }

            var module = _activeModuleTypes[moduleType];

            if (!_activeModules.ContainsKey(module))
            {
                _activeModules.Add(module, new HashSet<ITransitMod>());
            }

            var moduleOwners = _activeModules[module];
            moduleOwners.Add(mod);

            var modIsOwner = IsModOwnerOfModule(mod, module);
            if (modIsOwner)
            {
                module.AssetPath = mod.AssetPath;
            }
            module.SaveSettingsNeeded += mod.SaveSettings;

            if (modIsOwner)
            {
                return module;
            }
            else
            {
                return null;
            }
        }

        public void TryReleaseModules(ITransitMod mod)
        {
            if (!_modModuleTypes.ContainsKey(mod))
            {
                return;
            }

            var moduleTypes = _modModuleTypes[mod];

            foreach (var moduleType in moduleTypes)
            {
                TryReleaseModule(mod, moduleType);
            }
        }

        private void TryReleaseModule(ITransitMod mod, Type moduleType)
        {
            if (!_activeModuleTypes.ContainsKey(moduleType))
            {
                return;
            }

            var module = _activeModuleTypes[moduleType];

            if (_activeModules.ContainsKey(module))
            {
                var moduleOwners = _activeModules[module];
                if (moduleOwners.Contains(mod))
                {
                    moduleOwners.Remove(mod);
                    module.SaveSettingsNeeded -= mod.SaveSettings;
                }

                if (!moduleOwners.Any())
                {
                    Debug.Log(string.Format("TFW: Releasing module {0}", module.Name));
                    _activeModules.Remove(module);
                    _activeModuleTypes.Remove(moduleType);
                }
            }
            else
            {
                _activeModuleTypes.Remove(moduleType);
            }
        }
    }
}
