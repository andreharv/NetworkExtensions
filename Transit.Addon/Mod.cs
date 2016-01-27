using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.Steamworks;
using ICities;
using Transit.Framework.Modularity;

namespace Transit.Addon
{
    public sealed partial class Mod : ITransitMod
    {
        public string Name
        {
            get
            {
                OnGameLoaded();
                return _name;
            }
        }

        public string Description
        {
            get
            {
                OnGameLoaded();
                return _description;
            }
        }

        public string DefaultFolderPath
        {
            get { return _name; }
        }

        private ulong? _workshopId;
        public ulong WorkshopId
        {
            get
            {
                if (_workshopId == null)
                {
                    foreach(var mod in PluginManager.instance.GetPluginsInfo())
                    {
                        if (mod.userModInstance == this)
                        {
                            _workshopId = mod.publishedFileID.AsUInt64;
                        }
                    }
                    _workshopId = 543703997;
                }

                return _workshopId.Value;
            }
        }
    }
}
