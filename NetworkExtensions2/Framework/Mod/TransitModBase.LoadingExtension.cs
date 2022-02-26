using CitiesHarmony.API;
using ICities;
using NetworkExtensions2.Patching;
using System.Diagnostics;
using Transit.Framework.Modularity;
using Transit.Framework.Prerequisites;

namespace Transit.Framework.Mod {
    public abstract partial class TransitModBase : LoadingExtensionBase {
        public virtual void OnEnabled() {
            HarmonyHelper.DoOnHarmonyReady(Patcher.PatchAll);
            ModPrerequisites.InstallForMod(this);
            LoadModulesIfNeeded();
            LoadSettings();

            foreach (IModule module in Modules)
                module.OnEnabled();
        }

        public virtual void OnDisabled() {
            HarmonyHelper.DoOnHarmonyReady(Patcher.UnpatchAll);
            foreach (IModule module in Modules)
                module.OnDisabled();

            ModPrerequisites.UninstallForMod(this);
        }

        public override void OnLevelUnloading() {
            foreach (IModule module in Modules)
                module.OnLevelUnloading();
        }

        public override void OnLevelLoaded(LoadMode mode) {
            foreach (IModule module in Modules)
                module.OnLevelLoaded(mode);
        }
    }
}