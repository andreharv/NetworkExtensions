using Transit.Framework.Modularity;

namespace Transit.Addon.RealisticSpeed
{
    [Module("Transit.Mod.TAM")]
    public partial class RealisticSpeedModule : ModuleBase
    {
        public override string Name
        {
            get { return "Realistic Speed"; }
        }

        public override int Order
        {
            get { return 100; }
        }
    }
}
