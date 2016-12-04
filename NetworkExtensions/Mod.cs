using ColossalFramework;
using ColossalFramework.Plugins;
using Transit.Framework;
using Transit.Framework.Mod;

namespace NetworkExtensions
{
    public sealed partial class Mod : TransitModBase
    {
        public override string Name
        {
            get { return "Network Extensions"; }
        }

        public override string Description
        {
            get { return "An addition of highways and roads"; }
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
