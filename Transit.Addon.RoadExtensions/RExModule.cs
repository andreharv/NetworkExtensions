using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions
{
    [Module("Transit.Addon.Mod", "NetworkExtensions.Mod")]
    public partial class RExModule : ModuleBase
    {
        public const string REX_OBJECT_NAME = "Road Extensions";

        public const string ROAD_NETCOLLECTION = "Road";
        public const string REX_NETCOLLECTION = "TAM Road";
        public const string REX_PROPCOLLECTION = "TAM Prop";

        public override string Name
        {
            get { return "Roads"; }
        }

        public override int Order
        {
            get { return 1; }
        }
    }
}
