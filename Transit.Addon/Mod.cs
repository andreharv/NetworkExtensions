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
            get { return _name; }
        }

        public override string Description
        {
            get { return _description; }
        }
    }
}
