using System.Xml;
using ICities;

namespace Transit.Framework.Modularity
{
    public abstract partial class ModuleBase : IModule
    {
        private static int s_loadingId = 100;

        public abstract string Name { get; }

        private readonly int defaultOrder = s_loadingId++;
        public virtual int Order
        {
            get { return defaultOrder; }
        }

        public virtual bool IsEnabled { get; set; }

        public virtual void OnGameLoaded() { }

        public virtual void OnCreated(ILoading loading) { }

        public virtual void OnLevelLoaded(LoadMode mode) { }

        public virtual void OnLevelUnloading() { }

        public virtual void OnReleased() { }

        public virtual void OnEnabled() { }

        public virtual void OnDisabled() { }
    }
}
