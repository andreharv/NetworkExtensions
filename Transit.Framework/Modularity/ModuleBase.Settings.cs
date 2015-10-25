using System.Xml;
using ICities;

namespace Transit.Framework.Modularity
{
    public abstract partial class ModuleBase : IModule
    {
        public virtual void OnSettingsUI(UIHelperBase helper) { }

        public virtual void OnLoadSettings(XmlElement moduleElement) { }

        public virtual void OnSaveSettings(XmlElement moduleElement) { }

        public event SaveSettingsNeededEventHandler SaveSettingsNeeded;

        protected void FireSaveSettingsNeeded()
        {
            if (SaveSettingsNeeded != null)
            {
                SaveSettingsNeeded();
            }
        }
    }
}
