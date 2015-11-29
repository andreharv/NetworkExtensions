using System.Collections.Generic;
using System.Xml;
using ICities;
using Transit.Framework.Interfaces;

namespace Transit.Framework.Modularity
{
    public delegate void SaveSettingsNeededEventHandler();

    public interface IModule : IActivable, IIdentifiable, IOrderable
    {
        string AssetPath { get; set; }

        IEnumerable<IModulePart> Parts { get; }

        void OnGameLoaded();

        void OnCreated(ILoading loading);

        void OnLevelLoaded(LoadMode mode);

        void OnLevelUnloading();

        void OnReleased();

        void OnEnabled();

        void OnDisabled();

        void OnSettingsUI(UIHelperBase helper);

        void OnLoadSettings(XmlElement moduleElement);

        void OnSaveSettings(XmlElement moduleElement);

        event SaveSettingsNeededEventHandler SaveSettingsNeeded;
    }
}
