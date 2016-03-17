using System.Collections.Generic;
using Transit.Framework.Modularity;

namespace Transit.Framework.Mod
{
    public static class TransitModExtensions
    {
        public static void RegisterModules(this ITransitMod mod)
        {
            ModuleManager.instance.RegisterModules(mod);
        }

        public static IEnumerable<IModule> GetModules(this ITransitMod mod)
        {
            return ModuleManager.instance.GetModules(mod);
        }

        public static IEnumerable<IModule> GetOrCreateModules(this ITransitMod mod)
        {
            return ModuleManager.instance.GetOrCreateModules(mod);
        }

        public static void TryReleaseModules(this ITransitMod mod)
        {
            ModuleManager.instance.TryReleaseModules(mod);
        }
    }
}
