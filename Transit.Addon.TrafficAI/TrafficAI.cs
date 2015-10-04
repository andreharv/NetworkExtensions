using System;
using Transit.Framework.Modularity;
using Transit.Addon;

namespace Transit.Addon.TrafficAI
{
    [Module(Mod = typeof(Mod))]
    public partial class TrafficAIModule : ModuleBase
    {
        public override string Name
        {
            get { return "TrafficAI"; }
        }
    }
}
