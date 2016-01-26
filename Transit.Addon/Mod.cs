using Transit.Addon.Core;

namespace Transit.Addon
{
    public sealed partial class Mod : TransitModBase
    {
        public override ulong WorkshopId
        {
            get { return 543703997; }
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
