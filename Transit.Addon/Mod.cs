using ColossalFramework;
using ColossalFramework.Plugins;
using Transit.Framework.Mod;

namespace Transit.Addon
{
    public sealed partial class Mod : TransitModBase
    {
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
