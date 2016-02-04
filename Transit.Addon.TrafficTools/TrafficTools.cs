using System;
using Transit.Framework.Modularity;

namespace Transit.Addon.TrafficTools
{
    [Module("Transit.Addon.Mod")]
    public partial class TrafficToolsModule : ModuleBase
    {
        public override string Name
        {
            get { return "Traffic Tools"; }
        }
    }
}
