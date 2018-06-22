using ColossalFramework;
using ColossalFramework.Plugins;
using TransitPlus.Framework.Mod;
using Transit.Framework;

namespace NetworkExtensionsPlus
{
    public sealed partial class Mod : TransitModBase
    {
        public override string Name
        {
            get { return "NetworkExtensions2-Plus"; }
        }

        public override string Description
        {
            get { return "An addon to the successful Network Extensions Project which focuses on public transport and bike networks"; }
        }

        public override string Version
        {
            get { return "1.0.0"; }
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
