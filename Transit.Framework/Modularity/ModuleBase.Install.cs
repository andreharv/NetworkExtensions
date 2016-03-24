using ColossalFramework.Globalization;

namespace Transit.Framework.Modularity
{
    public abstract partial class ModuleBase : IModule
    {
        public virtual void OnInstallingLocalization(Locale locale) { }

        public virtual void OnInstallingAssets() { }

        public virtual void OnInstallingContent() { }
    }
}
