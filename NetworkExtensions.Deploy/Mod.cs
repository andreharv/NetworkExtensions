using ColossalFramework;
using ColossalFramework.Plugins;
using Transit.Framework;
using Transit.Framework.Modularity;

namespace NetworkExtensions
{
    public sealed partial class Mod : ITransitMod
    {
        public string Name
        {
            get
            {
                OnGameLoaded();
                return NAME;
            }
        }

        public string Description
        {
            get
            {
                OnGameLoaded();
                return DESCRIPTION;
            }
        }

        public ulong WorkshopId
        {
            get { return 478820060; }
        }

        private const string TAM_MOD_ID = "543703997";
        private bool? _isTAMInstalled;

        public bool IsTAMInstalled
        {
            get
            {
                if (_isTAMInstalled == null)
                {
                    _isTAMInstalled = Singleton<PluginManager>.instance.IsPluginInstalled(TAM_MOD_ID);
                }

                return _isTAMInstalled.Value;
            }
        }
    }
}
