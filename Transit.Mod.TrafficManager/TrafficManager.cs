using Transit.Framework.Mod;
using Transit.Framework.Prerequisites;

namespace Transit.Mod
{
    public sealed partial class TrafficManager : TransitModBase
    {
        public override ulong WorkshopId
        {
            get { return 583429740ul; }
        }

        public override string Name
        {
            get { return "Traffic Manager : President Edition"; }
        }

        public override string Description
        {
            get { return "Manage your city's traffic"; }
        }

        public override string Version
        {
            get { return "1.0.0"; }
        }

        public override PrerequisiteType Requirements
        {
            get { return PrerequisiteType.PathFinding; }
        }
    }
}
