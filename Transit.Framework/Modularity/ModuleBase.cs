using ICities;
using System;
using System.Xml;

namespace Transit.Framework.Modularity
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ModuleAttribute : Attribute
    {
        public Type Mod { get; set; }
    }

    public abstract class ModuleBase : IModule
    {
        public abstract string Name { get; }

        public bool IsEnabled { get; set; }

        public virtual void OnGameLoaded() { }

        public virtual void OnCreated(ILoading loading) { }

        public virtual void OnLevelLoaded(LoadMode mode) { }

        public virtual void OnLevelUnloading() { }

        public virtual void OnReleased() { }

        public virtual void OnEnabled() { }

        public virtual void OnDisabled() { }

        public virtual void OnSettingsUI(UIHelperBase helper) { }

        public virtual void OnLoadSettings(XmlElement moduleElement) { }

        public virtual void OnSaveSettings(XmlElement moduleElement) { }
    }
}
