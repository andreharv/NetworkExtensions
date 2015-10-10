using System.Collections.Generic;
using ICities;
using System;
using System.Xml;
using Transit.Framework.Interfaces;

namespace Transit.Framework.Modularity
{
    public interface IModule : IActivable, IIdentifiable
    {
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
    }
}
