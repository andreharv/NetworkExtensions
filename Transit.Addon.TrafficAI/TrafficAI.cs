using Transit.Framework.Modularity;

namespace Transit.Addon.TrafficAI
{
    [Module(Mod = typeof(Mod))]
    public partial class TrafficAIModule : ModuleBase
    {
        public override string Name
        {
            get { return "Traffic AI"; }
        }
    }
}
