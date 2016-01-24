using Transit.Framework.Modularity;

namespace Transit.Addon.TrafficAI
{
    [Module(typeof(Mod))]
    public partial class TrafficAIModule : ModuleBase
    {
        public override string Name
        {
            get { return "Pathfinding"; }
        }
    }
}
