using System;
using Transit.Framework.Modularity;
using Transit.Addon;

namespace Transit.Addon.TrafficAI
{
    [Module(Mod = typeof(Mod))]
    public partial class TrafficAIModule : ModuleBase
    {
        public TrafficAIModule(Mod mod)
        {
            this.Mod = mod;
        }

        public override string Name
        {
            get { return "Traffic AI"; }
        }

        public Mod Mod { get; private set; }
    }
}
