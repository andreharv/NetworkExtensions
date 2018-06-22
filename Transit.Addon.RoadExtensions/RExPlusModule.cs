using Transit.Framework.Modularity;

namespace TransitPlus.Addon.RoadExtensions
{
    [Module("NetworkExtensionsPlus.Mod")]
    public partial class RExPlusModule : ModuleBase
    {
        public const string RExPlusOBJECT_NAME = "Road Extensions Plus";

        public const string NET_COLLECTION_NAME = "TAM Road Plus";
        public const string PROP_COLLECTION_NAME = "TAM Prop Plus";

        public static string[] RequiredNetCollections
        {
            get { return new[] { "Road", "Expansion 2", "TAM Road" }; }
        }

        public override string Name
        {
            get { return "RoadsPlus"; }
        }

        public override int Order
        {
            get { return 1; }
        }
    }
}
