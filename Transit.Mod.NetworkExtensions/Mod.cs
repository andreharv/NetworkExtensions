using Transit.Framework.Mod;
using Transit.Framework.Prerequisites;

namespace Transit.Mod.NetworkExtensions
{
    public sealed partial class Mod : TransitModBase
    {
        public override ulong WorkshopId
        {
            get { return 478820060; }
        }

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

        public override PrerequisiteType Requirements
        {
            get { return PrerequisiteType.NetworkAI | PrerequisiteType.UI; }
        }
    }
}
