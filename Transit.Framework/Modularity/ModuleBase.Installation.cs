
namespace Transit.Framework.Modularity
{
    public abstract partial class ModuleBase : IModule
    {
        public virtual void OnInstallLocalization() { }

        public virtual void OnInstallAssets() { }
    }
}
