
namespace Transit.Framework.Modularity
{
    public abstract partial class ModuleBase : IModule
    {
        public virtual void OnInstallingLocalization() { }

        public virtual void OnInstallingAssets() { }

        public virtual void OnInstallingContent() { }
    }
}
