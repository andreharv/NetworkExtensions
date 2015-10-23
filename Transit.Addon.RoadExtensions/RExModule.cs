using System;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions
{
    [Module(Mod = typeof(Mod))]
    public partial class RExModule : ModuleBase
    {
        private const UInt64 WORKSHOP_ID = 478820060;

        public const string REX_OBJECT_NAME = "Road Extensions";

        public const string ROAD_NETCOLLECTION = "Road";
        public const string REX_NETCOLLECTION = "RExNetCollection";
        public const string REX_PROPCOLLECTION = "RExPropCollection";

        public override string Name
        {
            get { return "Road Extensions"; }
        }
    }
}
