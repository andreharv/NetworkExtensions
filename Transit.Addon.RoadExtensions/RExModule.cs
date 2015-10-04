using System;
using ICities;
using Transit.Framework.Modularity;
using Transit.Addon;

namespace Transit.Addon.RoadExtensions
{
    [Module(Mod = typeof(Mod))]
    public partial class RExModule : ModuleBase
    {
        private const UInt64 WORKSHOP_ID = 478820060;

        public const string REX_OBJECT_NAME = "Road Extensions";

        public const string ROAD_NETCOLLECTION = "Road";
        public const string REX_NETCOLLECTION = "REXCollection";

        public override string Name
        {
            get { return "Road Extensions"; }
        }

        public string Description
        {
            get { return "An addition of highways and roads"; }
        }
    }
}
