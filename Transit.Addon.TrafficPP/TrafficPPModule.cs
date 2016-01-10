using Transit.Framework.Modularity;

namespace Transit.Addon.TrafficPP
{
    [Module(typeof(Mod))]
    public partial class TrafficPPModule : ModuleBase
    {
        public override string Name
        {
            get { return "Traffic++"; }
        }

        // Hack For FileManager, deprecated
        public override string AssetPath { get { return InternalAssetPath; } set { InternalAssetPath = value; } }
        internal static string InternalAssetPath { get; private set; }
    }
}
