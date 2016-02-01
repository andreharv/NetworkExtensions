using System;
using Transit.Framework.Modularity;

namespace Transit.Addon.TrafficTools
{
    [Module(typeof(Mod))]
    public partial class TrafficToolsModule : ModuleBase
    {
        public override string Name
        {
            get { return "Traffic Tools"; }
        }
    }
}
