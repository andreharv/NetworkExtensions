using System;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions
{
    [Module("Transit.Addon.Mod", "NetworkExtensions.Mod")]
    public partial class RExModule : ModuleBase
    {
        public const string REX_OBJECT_NAME = "Road Extensions";

        public const string NET_COLLECTION_NAME = "TAM Road";
        public const string PROP_COLLECTION_NAME = "TAM Prop";

        public static string[] RequiredNetCollections
        {
            get { return new[] { "Road" }; }
        }

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
