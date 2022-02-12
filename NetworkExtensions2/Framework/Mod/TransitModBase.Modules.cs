using System;
using System.Collections.Generic;
using Transit.Addon.RoadExtensions;
using Transit.Addon.Tools;
using Transit.Framework.Modularity;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Framework.Mod {
    public partial class TransitModBase {
        private bool _modulesLoaded;
        private List<IModule> _modules = new List<IModule>();
        public IEnumerable<IModule> Modules {
            get { return _modules; }
        }

        protected void LoadModulesIfNeeded() {
            if (_modulesLoaded) {
                return;
            }

            try {
                LoadModule<RExModule>();
                LoadModule<ToolModule>();
            } catch (Exception ex) {
                Debug.Log("TFW: Crashed-Modules");
                Debug.Log("TFW: " + ex.Message);
                Debug.Log("TFW: " + ex.ToString());
            } finally {
                _modulesLoaded = true;
            }
        }

        private void LoadModule<M>() where M : IModule, new()  {
            try {
                var module = new M();
                Debug.Log(string.Format("TFW: Loading module {0}", module.Name));

                module.AssetPath = AssetPath;
                module.SaveSettingsNeeded += SaveSettings;
                _modules.Add(module);
            } catch (Exception ex) {
                Debug.Log("TFW: Crashed-Module " + typeof(M).Name);
                Debug.Log("TFW: " + ex.Message);
                Debug.Log("TFW: " + ex.ToString());
            }
        }
    }
}
