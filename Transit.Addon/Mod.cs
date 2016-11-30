using ColossalFramework;
using ColossalFramework.Plugins;
using Transit.Framework.Mod;

namespace Transit.Addon
{
    public sealed partial class Mod : TransitModBase
    {
        private ulong? _workshopId;
        public override ulong WorkshopId
        {
            get
            {
                if (_workshopId == null)
                {
                    foreach(var mod in Singleton<PluginManager>.instance.GetPluginsInfo())
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

        public override string Name
        {
            get { return "Transit Addons Mod"; }
        }

        public override string Description
        {
            get { return "Closed Beta"; }
        }

        public override string Version
        {
            get { return "0.0.1"; }
        }
    }
}
