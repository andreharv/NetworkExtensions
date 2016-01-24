using Transit.Framework.Modularity;

namespace Transit.Addon.TrafficTools
{
    [Module(typeof(Mod))]
    public partial class TrafficToolModule : ModuleBase
    {
        public override string Name
        {
            get { return "Tools"; }
        }

        // Hack For FileManager, deprecated
        public override string AssetPath { get { return InternalAssetPath; } set { InternalAssetPath = value; } }
        internal static string InternalAssetPath { get; private set; }
    }
}
